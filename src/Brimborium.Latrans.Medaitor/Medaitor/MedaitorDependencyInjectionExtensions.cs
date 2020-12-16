using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Medaitor;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection {
    public static class MedaitorDependencyInjectionExtensions {
        public static MediatorBuilder AddLatransMedaitor(this IServiceCollection services, Action<MediatorBuilder> configure=null) {
            
            var builder = new MediatorBuilder();
            services.AddScoped<IMedaitorAccess, MedaitorAccess>();
            services.AddScoped<IMedaitorClient, MedaitorClient>();
            services.AddSingleton<IMedaitorService>((sp)=>MedaitorService.Create(builder.Options));
            //builder.Services.AddTransient<IMedaitorClientConnected, MedaitorClientConnected>();

            //: 
            if (configure is object) {
                configure(builder);
            }
            return builder;
        }
    }
}
