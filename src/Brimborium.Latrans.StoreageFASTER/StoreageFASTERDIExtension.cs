using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using Brimborium.Latrans.EventLog;

using Microsoft.Extensions.DependencyInjection;
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
