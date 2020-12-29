using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Brimborium.Latrans.IO {
    public class EventLogStorageOptions { 
    }

    public interface IEventLogStorageFactory {
        Task<IEventLogStorage> CreateAsync(EventLogStorageOptions options);
    }
    public interface IEventLogStorage {
        Task WriteAsync(EventLogRecord eventLogRecord);
        Task ReadAsync(Action<EventLogRecord> callback);
    }
}
