using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Mediator;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection {
    public static class MediatorDependencyInjectionExtensions {
        public static MediatorBuilder AddLatransMedaitor(this IServiceCollection services, Action<MediatorBuilder> configure=null) {
            
            var builder = new MediatorBuilder();
            services.AddScoped<IMediatorAccess, MediatorAccess>();
            services.AddScoped<IMediatorClient, MediatorClient>();
            services.AddSingleton<IMediatorService>((sp)=>MediatorService.Create(builder.Options));
            //builder.Services.AddTransient<IMedaitorClientConnected, MedaitorClientConnected>();

            //: 
            if (configure is object) {
                configure(builder);
            }
            return builder;
        }
    }
}
