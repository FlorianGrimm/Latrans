#nullable enable

using Brimborium.Latrans.EventLog;
using Brimborium.Latrans.Storeage.FASTER;

namespace Microsoft.Extensions.DependencyInjection {
    public static class StoreageFASTERDIExtension {
        public static void AddEventLogCore(
            this IServiceCollection services
            ) {
            services.AddTransient<IEventLogStorageFactory, EventLogStorageFactory>();
        }
    }
}
