using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using BenchmarkDotNet.Attributes;

using Brimborium.Latrans.Collections;
using Brimborium.Latrans.EventLog;
using Brimborium.Latrans.Storeage;
using Brimborium.Latrans.Storeage.Readable;

namespace Benchmark {
    public class MemoryResizableStreamTest {
        private List<Dummy> lstWriteD;
        private List<EventLogRecord> lstWriteRL;
        private List<Dummy> lstReadD;
        private List<EventLogRecord> lstReadRL;
        private DateTime dt;
        private string txtWriteNewtonsoftStream;

        public MemoryResizableStreamTest() {
            lstWriteD = new List<Dummy>(cnt);
            lstWriteRL = new List<EventLogRecord>(cnt);
            lstReadD = new List<Dummy>(cnt);
            lstReadRL = new List<EventLogRecord>(cnt);
            dt = new DateTime(2000, 1, 1);
            txtWriteNewtonsoftStream = "";
        }

        [Params(6, 2000)]
        public int cnt;

        [GlobalSetup]
        public void GlobalSetup() {
            lstWriteD = new List<Dummy>(cnt);
            lstWriteRL = new List<EventLogRecord>(cnt);
            lstReadD = new List<Dummy>(cnt);
            lstReadRL = new List<EventLogRecord>(cnt);

            for (int idx = 1; idx < cnt; idx++) {
                var d = new Dummy() {
                    Id = idx,
                    A = idx.ToString(),
                    B = new Guid(idx, 1, 1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }),
                    C = dt.AddHours(idx)
                };
                lstWriteD.Add(d);
            }
        }

        [Benchmark]
        public void WriteNewtonsoftStream() {
            var sb = new StringBuilder(4096);
            using (var sw = new StringWriter(sb)) {
                for (int idx = 1; idx < cnt; idx++) {
                    var d = lstWriteD[idx - 1];
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(d);
                    var r = new EventLogRecord() {
                        LgId = (ulong)idx,
                        DT = dt.AddHours(idx),
                        Key = idx.ToString(),
                        TypeName = "D",
                        DataText = json
                    };
                    lstWriteRL.Add(r);
                    ReadableLogUtil.Write(r, sw);
                }
                sw.Close();
            }
            txtWriteNewtonsoftStream = sb.ToString();


            var log1 = sb.ToString();
#if output
            System.Console.Out.WriteLine(log1.Length);
#endif

            using (var sr = new StringReader(log1)) {
                ReadableLogUtil.Read(sr, (lstReadRL, lstReadD), (state, r) => {
                    state.lstReadRL.Add(r);
                    var d = Newtonsoft.Json.JsonConvert.DeserializeObject<Dummy>(r.DataText);
                    state.lstReadD.Add(d);
                });
            }
        }
#if false
        [Benchmark]
        public void WriteNewtonsoftStream2() {
            var sb = new StringBuilder(4096);
            using (var sw = new StringWriter(sb)) {
                for (int idx = 1; idx < cnt; idx++) {
                    var d = lstWriteD[idx - 1];
                    SimdJsonSharp.SimdJson
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(d);
                    var r = new ReadableLog() {
                        LgId = (ulong)idx,
                        DT = dt.AddHours(idx),
                        Key = idx.ToString(),
                        TypeName = "D",
                        Data = json
                    };
                    lstWriteRL.Add(r);
                    ReadableLogUtil.Write(r, sw);
                }
                sw.Close();
            }
            txtWriteNewtonsoftStream = sb.ToString();
        }
#endif

        public class Dummy {
            public int Id { get; set; }
            public string? A { get; set; }
            public Guid B { get; set; }
            public DateTime C { get; set; }
        }
    }
}
