using Brimborium.Latrans.Contracts;
using Brimborium.Latrans.EventLog;

using System;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Storeage.FASTER {
    public class EventLogStorageFactory
        : IEventLogStorageFactory {
        private readonly ISystemClock _SystemClock;

        public EventLogStorageFactory(ISystemClock systemClock) {
            this._SystemClock = systemClock;
        }

        public bool IsValidFor(EventLogStorageOptions options) {
            return string.Equals("FASTER", options.Implementation, StringComparison.OrdinalIgnoreCase);
        }

        public Task<IEventLogStorage?> CreateAsync(EventLogStorageOptions options) {
            return EventLogStorage.CreateAsync(options, this._SystemClock);
        }
    }
}
