using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Brimborium.Latrans.EventLog {
    public class EventLogStorageOptions {
        public string Implementation { get; set; }
        public string BaseFolder { get; set; }
    }

    public interface IEventLogStorageFactory {
        bool IsValidFor(EventLogStorageOptions options);
        Task<IEventLogStorage?> CreateAsync(EventLogStorageOptions options);
    }

    public interface IEventLogStorageDispatcher {
        Task<IEventLogStorage?> CreateAsync(EventLogStorageOptions options);
    }

    public interface IEventLogStorage {
        Task WriteAsync(EventLogRecord eventLogRecord);
        Task ReadAsync(Action<EventLogRecord> callback);
    }
}
