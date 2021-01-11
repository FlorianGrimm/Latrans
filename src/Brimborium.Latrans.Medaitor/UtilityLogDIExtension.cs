using Brimborium.Latrans.Contracts;
using Brimborium.Latrans.IO;
using Brimborium.Latrans.Utility;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection {
    public static class UtilityLogDIExtension {
        public static void AddLatransUtility(
            this IServiceCollection services
            ) {
            //if (!services.Any(d => d.ServiceType == typeof(UtilityLogDIExtensionLock))) {
            //    services.AddTransient<UtilityLogDIExtensionLock>();
            //}
            services.TryAddTransient<ISystemClock, SystemClock>();
            services.TryAddTransient<ILocalFileSystem, LocalFileSystem>();
            services.AddTransient<IJsonSerializerFacade, JsonSerializerSystemTextJsonFacade>();
        }

        //private class UtilityLogDIExtensionLock { }
    }
}
