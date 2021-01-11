using Brimborium.Latrans.Contracts;
using Brimborium.Latrans.EventLog;
using Brimborium.Latrans.IO;
using Brimborium.Latrans.Utility;

using System;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Storeage.Readable {
    public class EventLogStorageFactory
        : IEventLogStorageFactory {
        private readonly IJsonSerializerFacade _JsonSerializerFacade;
        private readonly ISystemClock _SystemClock;
        private readonly ILocalFileSystem _LocalFileSystem;

        public EventLogStorageFactory(
            IJsonSerializerFacade jsonSerializerFacade,
            ISystemClock systemClock,
            ILocalFileSystem localFileSystem
            ) {
            this._JsonSerializerFacade = jsonSerializerFacade;
            this._SystemClock = systemClock;
            this._LocalFileSystem = localFileSystem;
        }

        public bool IsValidFor(EventLogStorageOptions options) {
            return string.IsNullOrEmpty(options.Implementation)
                || string.Equals("Readable", options.Implementation, StringComparison.OrdinalIgnoreCase);
            //return string.IsNullOrEmpty(options.BaseFolder);
        }

        public Task<IEventLogStorage?> CreateAsync(EventLogStorageOptions options) {
            return EventLogStorage.CreateAsync(options, this._JsonSerializerFacade, this._LocalFileSystem, this._SystemClock);
        }
    }
}
