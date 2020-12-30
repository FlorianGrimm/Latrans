using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using Brimborium.Latrans.EventLog;

using Brimborium.Latrans.Storeage.Readable;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
            //services.TryAddEventLogCore();
            services.AddTransient<IEventLogStorageFactory, EventLogStorageFactory>();
        }
    }
}
