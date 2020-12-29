using System;

namespace Brimborium.Latrans.Collections {
    public struct ReadableLog {
        public ulong LgId { get; set; }
        public string Key { get; set; }
        public string TypeName { get; set; }
        public string Data { get; set; }
        public DateTime DT { get; set; }
    }
}

