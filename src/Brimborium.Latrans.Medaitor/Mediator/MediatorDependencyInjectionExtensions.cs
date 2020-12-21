using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Mediator;
using Brimborium.Latrans.Utility;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection {
    public static class MediatorDependencyInjectionExtensions {
        public static MediatorBuilder AddLatransMedaitor(this IServiceCollection services, Action<IMediatorBuilder>? configure = null) {
            var builder = new MediatorBuilder();
            services.AddScoped<ILocalDisposables, LocalDisposables>();
            services.AddScoped<IMediatorClientFactory, MediatorClientFactory>();
            services.AddTransient<IMediatorClient, MediatorClient>();
            services.AddSingleton<IMediatorService>((serviceProvider) => {
                IMediatorService result;
                lock (serviceProvider) {
                    builder.Build();
                    var options = builder.GetOptions();
                    result = new MediatorService(options);
                }
                return result;
            });
            if (configure is object) {
                configure(builder);
            }
            return builder;
        }
    }

}
