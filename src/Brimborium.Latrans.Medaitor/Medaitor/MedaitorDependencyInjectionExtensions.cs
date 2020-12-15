using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Medaitor;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection {
    public static class MedaitorDependencyInjectionExtensions {
        public static void AddMedaitor(this IServiceCollection services) {
            services.AddScoped<IMedaitorAccess, MedaitorAccess>();
        }

        public static void AddHandler<T>(this IServiceCollection services, RequestRelatedTypes requestRelatedTypes) {
            Type handlerType = typeof(T);
            var interfaces = handlerType.GetInterfaces();
            foreach (var @interface in interfaces) {
                if (@interface.IsGenericType) {
                    var genericTypeDefinition = @interface.GetGenericTypeDefinition();
                    if (typeof(Brimborium.Latrans.Activity.IActivityHandler<,>).Equals(genericTypeDefinition)) {
                        services.AddTransient(genericTypeDefinition, handlerType);
                        var genericTypeArguments = @interface.GenericTypeArguments;
                        var requestType = genericTypeArguments[0];
                        if (requestRelatedTypes.TryGetValue(requestType, out var knownRRT)) {
                            knownRRT.AddHandlerType(handlerType);
                        } else {
                            var responseType = genericTypeArguments[1];
                            //var typeIActivityContext1 = typeof(Brimborium.Latrans.Activity.IActivityContext<>).MakeGenericType(requestType);
                            //var typeIActivityContext2 = typeof(Brimborium.Latrans.Activity.IActivityContext<,>).MakeGenericType(requestType, responceType);
                            var activityContextType2 = typeof(Brimborium.Latrans.Activity.ActivityContext<,>).MakeGenericType(requestType, responseType);
                            requestRelatedTypes.Add(new RequestRelatedType(requestType, responseType, handlerType, activityContextType2));
                            services.AddTransient(activityContextType2, activityContextType2);
                        }
                    }
                }
            }
            services.AddTransient(handlerType, handlerType);
        }
    }
}
