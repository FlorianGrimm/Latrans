using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Brimborium.Latrans.Contracts;
using Brimborium.Latrans.EventLog;
using Brimborium.Latrans.IO;
using Brimborium.Latrans.Utility;

using Microsoft.Extensions.DependencyInjection;
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
        }
        
        //private class UtilityLogDIExtensionLock { }
    }
}
