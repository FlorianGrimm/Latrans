using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Medaitor;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection {
    public static class MedaitorDependencyInjectionExtensions {
        public static MediatorBuilder AddLatransMedaitor(this IServiceCollection services, Action<MediatorOptions> configure=null) {
            var options = new MediatorOptions();
            var builder = new MediatorBuilder(options);
            services.AddScoped<IMedaitorAccess, MedaitorAccess>();
            services.AddScoped<IMedaitorClient, MedaitorClient>();
            services.AddSingleton<IMedaitorService>((sp)=>MedaitorService.Create(options));
            if (configure is object) {
                configure(options);
            }
            return builder;
        }
    }
}
