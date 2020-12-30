using Brimborium.Latrans.Contracts;
using Brimborium.Latrans.EventLog;

using System;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Storeage.Readable {
    public class EventLogStorageFactory
        : IEventLogStorageFactory {
        private readonly ISystemClock _SystemClock;

        public EventLogStorageFactory(ISystemClock systemClock) {
            this._SystemClock = systemClock;
        }

        public bool IsValidFor(EventLogStorageOptions options) {
            return string.IsNullOrEmpty(options.Implementation)
                || string.Equals("Readable", options.Implementation, StringComparison.OrdinalIgnoreCase);
            //return string.IsNullOrEmpty(options.BaseFolder);
        }

        public Task<IEventLogStorage?> CreateAsync(EventLogStorageOptions options) {
            return EventLogStorage.CreateAsync(options, this._SystemClock);
        }
    }
}
