using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using BenchmarkDotNet.Attributes;

using Brimborium.Latrans.Collections;
using Brimborium.Latrans.IO;

using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;

namespace Benchmark {
    public class JsonTest {
        private List<Dummy> lstWriteDummy;
        //private List<EventLogRecord> lstWriteRL;
        //private List<Dummy> lstReadD;
        //private List<EventLogRecord> lstReadRL;
        private DateTime dt;
        //private string txtWriteNewtonsoftStream;

        public JsonTest() {
            lstWriteDummy = new List<Dummy>(cnt);
            //lstWriteRL = new List<EventLogRecord>(cnt);
            //lstReadD = new List<Dummy>(cnt);
            //lstReadRL = new List<EventLogRecord>(cnt);
            dt = new DateTime(2000, 1, 1);
            //txtWriteNewtonsoftStream = "";
        }
        [GlobalSetup]
        public void GlobalSetup() {
            lstWriteDummy = new List<Dummy>(cnt);
            //lstWriteRL = new List<EventLogRecord>(cnt);
            //lstReadD = new List<Dummy>(cnt);
            //lstReadRL = new List<EventLogRecord>(cnt);

            for (int idx = 1; idx < cnt; idx++) {
                var d = new Dummy() {
                    Id = idx,
                    A = idx.ToString(),
                    B = new Guid(idx, 1, 1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }),
                    C = dt.AddHours(idx)
                };
                lstWriteDummy.Add(d);
            }
        }

        [Params(6, 2000)]
        public int cnt;

        [Benchmark]
        public void NewtonsoftReadWrite() {
            //MemoryResizableStream memoryResizableStream = new MemoryResizableStream();
            //using (var sw = new StreamWriter(memoryResizableStream)) {
            for (int idx = 1; idx < cnt; idx++) {
                var d = lstWriteDummy[idx - 1];

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(d);
                if (json.Length < 10) { throw new Exception(); }
                //var r = new EventLogRecord() {
                //    LgId = (ulong)idx,
                //    DT = dt.AddHours(idx),
                //    Key = idx.ToString(),
                //    TypeName = "D",
                //    DataText = json
                //};
                ////lstWriteRL.Add(r);
                //ReadableLogUtil.Write(r, sw);
                var d2 = Newtonsoft.Json.JsonConvert.DeserializeObject<Dummy>(json);
                if (d.Id != d2.Id) { throw new Exception(); }
            }
            //    sw.Close();
            //}
        }
        [Benchmark]
        public void Utf8JsonReadWrite() {
            //MemoryResizableStream memoryResizableStream = new MemoryResizableStream();
            //using (var sw = new StreamWriter(memoryResizableStream)) {
            for (int idx = 1; idx < cnt; idx++) {
                var d = lstWriteDummy[idx - 1];

                var json = Utf8Json.JsonSerializer.Serialize<Dummy>(d);
                if (json.Length < 10) { throw new Exception(); }
                //var r = new EventLogRecord() {
                //    LgId = (ulong)idx,
                //    DT = dt.AddHours(idx),
                //    Key = idx.ToString(),
                //    TypeName = "D",
                //    DataText = json
                //};
                ////lstWriteRL.Add(r);
                //ReadableLogUtil.Write(r, sw);
                var d2 = Utf8Json.JsonSerializer.Deserialize<Dummy>(json);
                if (d.Id != d2.Id) { throw new Exception(); }
            }
            //    sw.Close();
            //}

        }
        public class Dummy {
            public int Id { get; set; }
            public string A { get; set; }
            public Guid B { get; set; }
            public DateTime C { get; set; }
        }
    }
}
