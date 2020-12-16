using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Brimborium.Latrans.Activity;

using Microsoft.Extensions.DependencyInjection;
namespace Brimborium.Latrans.Medaitor {
    public class MediatorOptions {
        public readonly IServiceCollection ServicesMediator;
        public readonly RequestRelatedTypes RequestRelatedTypes;

        public MediatorOptions() {
            this.RequestRelatedTypes = new RequestRelatedTypes();
            this.ServicesMediator = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        }
    }

    public class MediatorBuilder {
        private readonly MediatorOptions _Options;
        private readonly IServiceCollection _Services;
        private readonly RequestRelatedTypes _RequestRelatedTypes;

        public MediatorBuilder(MediatorOptions options) {
            this._Options = options ?? throw new ArgumentNullException(nameof(options));
            this._Services = options.ServicesMediator;
            this._RequestRelatedTypes = options.RequestRelatedTypes;
        }

        public MediatorOptions Options => this._Options;
        public void AddHandler<THandler>() {
            Type handlerType = typeof(THandler);
            var interfaces = handlerType.GetInterfaces();
            foreach (var @interface in interfaces) {
                if (@interface.IsGenericType) {
                    var genericTypeDefinition = @interface.GetGenericTypeDefinition();
                    if (typeof(Brimborium.Latrans.Activity.IActivityHandler<,>).Equals(genericTypeDefinition)) {
                        this._Services.AddTransient(@interface, handlerType);
                        var genericTypeArguments = @interface.GenericTypeArguments;
                        var requestType = genericTypeArguments[0];
                        var responseType = genericTypeArguments[1];
                        //
                        var activityHandler1Type = typeof(Brimborium.Latrans.Activity.IActivityHandler<>).MakeGenericType(requestType);
                        this._Services.AddTransient(activityHandler1Type, handlerType);
                        var activityHandler2Type = typeof(Brimborium.Latrans.Activity.IActivityHandler<,>).MakeGenericType(requestType, responseType);
                        this._Services.AddTransient(activityHandler2Type, handlerType);
                        //
                        if (this._RequestRelatedTypes.TryGetValue(requestType, out var knownRRT)) {
                            knownRRT.AddHandlerType(handlerType);
                        } else {
                            //var typeIActivityContext1 = typeof(Brimborium.Latrans.Activity.IActivityContext<>).MakeGenericType(requestType);
                            //var typeIActivityContext2 = typeof(Brimborium.Latrans.Activity.IActivityContext<,>).MakeGenericType(requestType, responceType);

                            //var activityContext1Type = typeof(Brimborium.Latrans.Activity.IActivityContext<>).MakeGenericType(requestType);
                            //this._Services.AddTransient(activityContext1Type, activityContext1Type);

                            var activityContextType = typeof(Brimborium.Latrans.Medaitor.MedaitorContext<,>).MakeGenericType(requestType, responseType);
                            this._Services.AddTransient(activityContextType, activityContextType);
                            Func<CreateActivityContextArguments, object, IActivityContext> createActivityContext
                                = (Func<CreateActivityContextArguments, object, IActivityContext>)activityContextType
                                .GetMethod("GetCreateInstance", BindingFlags.Public | BindingFlags.Static)
                                .Invoke(null, null);

                            this._RequestRelatedTypes.Add(
                                new RequestRelatedType(
                                    requestType,
                                    responseType,
                                    handlerType,
                                    activityContextType,
                                    createActivityContext));
                        }
                    }
                }
            }
            this._Services.AddTransient(handlerType, handlerType);
        }
    }
}
