using BenchmarkDotNet.Attributes;

using Brimborium.Latrans.EventLog;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark {
    public class EventLogStorageTest {
        private List<Dummy> lstWriteDummy;
        private DateTime dt;
        private string latransWrite2Logs;

        [Params(100, 1000, 10_000)]
        public int cnt;

        public EventLogStorageTest() {
            lstWriteDummy = new List<Dummy>(cnt);
            dt = new DateTime(2000, 1, 1);
            latransWrite2Logs = string.Empty;
        }

        [GlobalSetup]
        public void GlobalSetup() {
            lstWriteDummy = new List<Dummy>(cnt);

            for (int idx = 1; idx < cnt; idx++) {
                var d = new Dummy() {
                    Id = idx,
                    A = idx.ToString(),
                    B = new Guid(idx, 1, 1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }),
                    C = dt.AddMilliseconds(idx)
                };
                lstWriteDummy.Add(d);
            }

            var tempPath = System.IO.Path.GetTempPath();
            var latransWrite2Logs = System.IO.Path.Combine(tempPath, "LatransWrite2Logs");
            this.latransWrite2Logs = latransWrite2Logs;

            if (!System.IO.Directory.Exists(latransWrite2Logs)) {
                System.IO.Directory.CreateDirectory(latransWrite2Logs);
            }

            var filesToDelete = System.IO.Directory.EnumerateFiles(latransWrite2Logs).ToArray();
            foreach (var fileToDelete in filesToDelete) {
                System.IO.File.Delete(fileToDelete);
            }
        }


        [Benchmark]
        public async Task EventLogReadable() {

            if (!System.IO.Directory.Exists(latransWrite2Logs)) {
                System.IO.Directory.CreateDirectory(latransWrite2Logs);
            }

            var filesToDelete = System.IO.Directory.EnumerateFiles(latransWrite2Logs).ToArray();
            foreach (var fileToDelete in filesToDelete) {
                System.IO.File.Delete(fileToDelete);
            }

            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddEventLogReadable();
            using (var serviceProvider = serviceCollection.BuildServiceProvider()) {
                var factory = serviceProvider.GetRequiredService<IEventLogStorageFactory>();
                var eventLogStorage = await factory.CreateAsync(new EventLogStorageOptions() {
                    BaseFolder = latransWrite2Logs,
                    Implementation = "Readable"
                });
                if (eventLogStorage is null) {
                    throw new InvalidOperationException("eventLogStorage is null");
                }

                for (int idx = 0; idx < lstWriteDummy.Count; idx++) {
                    eventLogStorage.Write(new EventLogRecord() {
                        LgId = (ulong)idx + 1,
                        DT = dt.AddMilliseconds(idx),
                        Key = (idx + 1).ToString(),
                        TypeName = "Dummy",
                        DataObject = lstWriteDummy[idx]
                    });
                }
                eventLogStorage.Dispose();
            }
        }

        [DataContract]
        public class Dummy {
            [DataMember(Order = 1)]
            public int Id { get; set; }
            [DataMember(Order = 2)]
            public string? A { get; set; }
            [DataMember(Order = 3)]
            public Guid B { get; set; }
            [DataMember(Order = 4)]
            public DateTime C { get; set; }
        }
    }
}
