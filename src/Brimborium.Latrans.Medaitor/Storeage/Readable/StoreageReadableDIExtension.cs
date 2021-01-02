using Brimborium.Latrans.EventLog;
using Brimborium.Latrans.Storeage.Readable;

using System.Linq;

namespace Microsoft.Extensions.DependencyInjection {
    public static class StoreageReadableDIExtension {
        public static void TryAddEventLogReadable(
            this IServiceCollection services
            ) {
            if (!services.Any(d => d.ImplementationType == typeof(EventLogStorageFactory))) {
                services.AddEventLogReadable();
            }
        }

        public static void AddEventLogReadable(
            this IServiceCollection services
            ) {
            services.TryAddEventLogCore();
            services.AddTransient<IEventLogStorageFactory, EventLogStorageFactory>();
        }
    }
}
