#nullable enable

using Brimborium.Latrans.EventLog;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using Xunit;

namespace Brimborium.Latrans.Medaitor.Test.Storeage.Readable {
    public class EventLogStorageTests {
        [Fact]
        public void NewLineTest() {
            var d = new Dummy() {
                A = "1\n2"
            };

            var json = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes<Dummy>(d);
            Assert.DoesNotContain((byte)'\n', json);
        }

        [Fact]
        public async Task Write2Logs() {
            var dt = new DateTime(2000, 1, 1);
            var tempPath = System.IO.Path.GetTempPath();
            var latransWrite2Logs = System.IO.Path.Combine(tempPath, "LatransWrite2Logs");
            if (!System.IO.Directory.Exists(latransWrite2Logs)) {
                System.IO.Directory.CreateDirectory(latransWrite2Logs);
            }

            var filesToDelete =  System.IO.Directory.EnumerateFiles(latransWrite2Logs).ToArray();
            foreach (var fileToDelete in filesToDelete) {
                System.IO.File.Delete(fileToDelete);
            }

            int cnt = 10;
            var lstWriteDummy = new List<Dummy>(cnt);

            for (int idx = 1; idx < cnt; idx++) {
                var d = new Dummy() {
                    Id = idx,
                    A = idx.ToString(),
                    B = new Guid(idx, 1, 1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }),
                    C = dt.AddHours(idx)
                };
                lstWriteDummy.Add(d);
            }

            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddEventLogReadable();
            using (var serviceProvider = serviceCollection.BuildServiceProvider()) {
                var factory = serviceProvider.GetRequiredService<IEventLogStorageFactory>();
                var eventLogStorage = await factory.CreateAsync(new EventLogStorageOptions() {
                    BaseFolder = latransWrite2Logs,
                    Implementation = "Readable"
                });
                if (eventLogStorage is null) { throw new Exception(); }
                for (int idx = 0; idx < lstWriteDummy.Count; idx++) {
                    eventLogStorage.Write(new EventLogRecord() {
                        LgId = (ulong)idx,
                        Key = (idx + 1).ToString(),
                        TypeName = "Dummy",
                        DataObject = lstWriteDummy[idx]
                    });
                    //await Task.Delay(100);
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
