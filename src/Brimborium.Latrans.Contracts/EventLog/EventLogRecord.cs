using System;

namespace Brimborium.Latrans.EventLog {
    public struct EventLogRecord {
        public ulong LgId { get; set; }
        public string? Key { get; set; }
        public string? TypeName { get; set; }
        public string? DataText { get; set; }
        public byte[]? DataByte { get; set; }
        public object? DataObject { get; set; }
        public DateTime DT { get; set; }
    }
}

