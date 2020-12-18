using Brimborium.Latrans.Activity;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Reflection;

namespace Brimborium.Latrans.Mediator {
    public class MediatorBuilder {
        private readonly IServiceCollection _Services;
        private readonly RequestRelatedTypes _RequestRelatedTypes;
        
        public MediatorBuilder() {
            this._Services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            this._RequestRelatedTypes = new RequestRelatedTypes();
            this._Services.AddOptions();
            this._Services.AddOptions<ActivityWaitForSpecificationOptions>();
        }

        public IServiceCollection Services => this._Services;
        public RequestRelatedTypes RequestRelatedTypes => this._RequestRelatedTypes;

        internal MediatorOptions GetOptions() {
            this._Services.AddSingleton<ActivityWaitForSpecificationDefaults>();

            return new MediatorOptions(
                this._Services,
                this._RequestRelatedTypes
                );
        }

        public MediatorBuilder AddHandler<THandler>() {
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

                            var activityContextType = typeof(Brimborium.Latrans.Mediator.MediatorContext<,>).MakeGenericType(requestType, responseType);
                            this._Services.AddTransient(activityContextType, activityContextType);
                            //var createActivityContext
                            //    = (Func<CreateActivityContextArguments, object, IActivityContext>)activityContextType
                            //    .GetMethod("GetCreateInstance", BindingFlags.Public | BindingFlags.Static)
                            //    .Invoke(null, null);

                            var factoryActivityContext = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateFactory(activityContextType, new Type[] { typeof(CreateActivityContextArguments), requestType });

                            var medaitorClientConnectedType = typeof(Brimborium.Latrans.Mediator.MediatorClientConnected<,>).MakeGenericType(requestType, responseType);
                            //var createClientConnected
                            //    = (Func<CreateClientConnectedArguments, object, IMediatorClientConnected>)medaitorClientConnectedType
                            //    .GetMethod("GetCreateInstance", BindingFlags.Public | BindingFlags.Static)
                            //    .Invoke(null, null);
                            var factoryClientConnected = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateFactory(medaitorClientConnectedType, new Type[] { typeof(CreateClientConnectedArguments), requestType });

                            
                            this._RequestRelatedTypes.Add(
                                new RequestRelatedType(
                                    requestType,
                                    responseType,
                                    handlerType,
                                    activityContextType,
                                    factoryActivityContext,
                                    factoryClientConnected));
                        }
                    }
                }
            }
            this._Services.AddTransient(handlerType, handlerType);
            return this;
        }
    }
}
