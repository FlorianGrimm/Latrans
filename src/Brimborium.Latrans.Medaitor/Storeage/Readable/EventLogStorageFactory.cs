using Brimborium.Latrans.Contracts;
using Brimborium.Latrans.EventLog;
using Brimborium.Latrans.IO;

using System;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Storeage.Readable {
    public class EventLogStorageFactory
        : IEventLogStorageFactory {
        private readonly ISystemClock _SystemClock;
        private readonly ILocalFileSystem _LocalFileSystem;

        public EventLogStorageFactory(
            ISystemClock systemClock,
            ILocalFileSystem localFileSystem
            ) {
            this._SystemClock = systemClock;
            this._LocalFileSystem = localFileSystem;
        }

        public bool IsValidFor(EventLogStorageOptions options) {
            return string.IsNullOrEmpty(options.Implementation)
                || string.Equals("Readable", options.Implementation, StringComparison.OrdinalIgnoreCase);
            //return string.IsNullOrEmpty(options.BaseFolder);
        }

        public Task<IEventLogStorage?> CreateAsync(EventLogStorageOptions options) {
            return EventLogStorage.CreateAsync(options, this._LocalFileSystem, this._SystemClock);
        }
    }
}
