using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;

using Xunit;
using System.Runtime.Serialization;

namespace Brimborium.Latrans.Medaitor.Test.Storeage.Readable {
public    class EventLogStorageTests {
        [Fact]
        public void NewLineTest() {
            var d = new Dummy() { 
                A="1\n2"
            };
            var json=Utf8Json.JsonSerializer.ToJsonString<Dummy>(d);
            Assert.False(json.Contains('\n'));
            //Utf8Json.JsonSerializer.Serialize<Dummy>
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
