using BenchmarkDotNet.Attributes;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
        [Benchmark]
        public void SystemTextJsonReadWrite() {
            //MemoryResizableStream memoryResizableStream = new MemoryResizableStream();
            //using (var sw = new StreamWriter(memoryResizableStream)) {
            for (int idx = 1; idx < cnt; idx++) {
                var d = lstWriteDummy[idx - 1];

                var json = System.Text.Json.JsonSerializer.Serialize<Dummy>(d);
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
                var d2 = System.Text.Json.JsonSerializer.Deserialize<Dummy>(json);
                if (d2 is null || d.Id != d2.Id) { throw new Exception(); }
            }
            //    sw.Close();
            //}
        }

        [Benchmark]
        public void MessagePackReadWrite() {
            var resolver = MessagePack.Resolvers.CompositeResolver.Create(
                new[] { MessagePack.Formatters.TypelessFormatter.Instance },
                new[] { MessagePack.Resolvers.StandardResolver.Instance });
            var options = MessagePack.MessagePackSerializerOptions.Standard;


            //MemoryResizableStream memoryResizableStream = new MemoryResizableStream();
            //using (var sw = new StreamWriter(memoryResizableStream)) {
            for (int idx = 1; idx < cnt; idx++) {
                var d = lstWriteDummy[idx - 1];
                var json = MessagePack.MessagePackSerializer.Serialize<Dummy>(d, options);
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
                var d2 = MessagePack.MessagePackSerializer.Deserialize<Dummy>(json, options);
                if (d2 is null || d.Id != d2.Id) { throw new Exception(); }
            }
            //    sw.Close();
            //}
        }


        [Benchmark]
        public void MessagePackLZ4ReadWrite() {
            var resolver = MessagePack.Resolvers.CompositeResolver.Create(
                new[] { MessagePack.Formatters.TypelessFormatter.Instance },
                new[] { MessagePack.Resolvers.StandardResolver.Instance });
            var options = MessagePack.MessagePackSerializerOptions.Standard
                .WithOmitAssemblyVersion(true)
                .WithCompression(MessagePack.MessagePackCompression.Lz4Block)
                .WithResolver(resolver)
                ;


            //MemoryResizableStream memoryResizableStream = new MemoryResizableStream();
            //using (var sw = new StreamWriter(memoryResizableStream)) {
            for (int idx = 1; idx < cnt; idx++) {
                var d = lstWriteDummy[idx - 1];
                var json = MessagePack.MessagePackSerializer.Serialize<Dummy>(d, options);
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
                var d2 = MessagePack.MessagePackSerializer.Deserialize<Dummy>(json, options);
                if (d2 is null || d.Id != d2.Id) { throw new Exception(); }
            }
            //    sw.Close();
            //}
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
