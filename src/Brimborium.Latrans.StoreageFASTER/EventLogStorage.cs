#nullable enable

using Brimborium.Latrans.Contracts;
using Brimborium.Latrans.EventLog;
using Brimborium.Latrans.Utility;

using System;
using System.IO;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Storeage.FASTER {
    public class EventLogStorage
        : IEventLogStorage {
        public static async Task<IEventLogStorage?> CreateAsync(
            EventLogStorageOptions options,
            ISystemClock? systemClock = default) {
            var result = new EventLogStorage(options, systemClock);
            await result.InitializeAsync();
            return result;
        }

        private string _BaseFolder;
        private readonly ISystemClock _SystemClock;

        public EventLogStorage(EventLogStorageOptions options, ISystemClock? systemClock = default) {
            this._BaseFolder = options.BaseFolder;
            this._SystemClock = systemClock ?? new SystemClock();
        }

        public Task InitializeAsync() {
            if (System.IO.Directory.Exists(this._BaseFolder)) {
                return Task.CompletedTask;
            } else {
                System.IO.Directory.CreateDirectory(this._BaseFolder);
                return Task.CompletedTask;
            }
        }

        public Task ReadAsync(Action<EventLogRecord> callback) {
            throw new NotImplementedException();
        }


        public Task WriteAsync(EventLogRecord eventLogRecord) {
            throw new NotImplementedException();
        }
    }
}
