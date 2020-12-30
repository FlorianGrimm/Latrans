using Brimborium.Latrans.EventLog;

using System.Linq;

namespace Microsoft.Extensions.DependencyInjection {
    public static class EventLogDIExtension {
        public static void TryAddEventLogCore(
            this IServiceCollection services
            ) {
            if (!services.Any(d => d.ServiceType == typeof(IEventLogStorageDispatcher))) {
                services.AddEventLogCore();
            }
        }

        public static void AddEventLogCore(
            this IServiceCollection services
            ) {
            services.AddTransient<IEventLogStorageDispatcher, EventLogStorageDispatcher>();
        }
    }
}
