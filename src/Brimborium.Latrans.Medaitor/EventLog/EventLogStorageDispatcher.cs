using System.Collections.Generic;
using System.Threading.Tasks;

namespace Brimborium.Latrans.EventLog {
    public sealed class EventLogStorageDispatcher
        : IEventLogStorageDispatcher {
        private readonly List<IEventLogStorageFactory> _Factories;

        public EventLogStorageDispatcher(
            IEnumerable<IEventLogStorageFactory> factories
            ) {
            this._Factories = new List<IEventLogStorageFactory>(factories);
        }

        public List<IEventLogStorageFactory> Factories => this._Factories;

        public async Task<IEventLogStorage?> CreateAsync(EventLogStorageOptions options) {
            for (int idx = 0; idx < this._Factories.Count; idx++) {
                var factory = this._Factories[idx];
                if (factory.IsValidFor(options)) {
                    var result = await factory.CreateAsync(options);
                    return result;
                }
            }
            return null;
        }
    }
}