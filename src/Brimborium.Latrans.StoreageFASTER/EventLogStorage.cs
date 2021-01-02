#nullable enable

using Brimborium.Latrans.Contracts;
using Brimborium.Latrans.EventLog;
using Brimborium.Latrans.Utility;

using FASTER.core;

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

        private readonly string _BaseFolder;
        private readonly ISystemClock _SystemClock;
        private FasterLog _Log;

        public EventLogStorage(EventLogStorageOptions options, ISystemClock? systemClock = default) {
            this._BaseFolder = options.BaseFolder;
            this._SystemClock = systemClock ?? new SystemClock();
        }

        public async Task InitializeAsync() {
            if (!System.IO.Directory.Exists(this._BaseFolder)) {
                System.IO.Directory.CreateDirectory(this._BaseFolder);
            }
            FasterLogSettings logSettings = new FasterLogSettings();
            this._Log = await FasterLog.CreateAsync(logSettings);
        }

        public void Write(EventLogRecord eventLogRecord) {
            //this._Log.Enqueue()
            throw new NotImplementedException();
        }

        public Task ReadAsync(Action<EventLogRecord> callback) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}
