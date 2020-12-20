using Brimborium.Latrans.Activity;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Brimborium.Latrans.Mediator {
    public class MediatorBuilder : IMediatorBuilder {
        private readonly IServiceCollection _Services;
        private readonly RequestRelatedTypes _RequestRelatedTypes;
        public readonly List<BuildRequestRelatedType> ActivityHandlers;
        public readonly List<BuildRequestRelatedType> DispatchActivityHandlers;

        public MediatorBuilder() {
            this._Services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            this._RequestRelatedTypes = new RequestRelatedTypes();
            this._Services.AddOptions();
            this._Services.AddOptions<ActivityWaitForSpecificationOptions>();
            this.ActivityHandlers = new List<BuildRequestRelatedType>();
            this.DispatchActivityHandlers = new List<BuildRequestRelatedType>();
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

        //

        public IMediatorBuilder AddActivityHandler<THandler>()
            where THandler : IActivityHandler {
            this.AddHandlerType(typeof(THandler));
            return this;
        }

        public IMediatorBuilder AddDispatchHandler<THandler>()
            where THandler : IDispatchActivityHandler {
            this.AddHandlerType(typeof(THandler));
            return this;
        }


        public IMediatorBuilder AddHandlerType(Type handlerType) {
            if (handlerType is null) { throw new ArgumentNullException(nameof(handlerType)); }
            //
            var interfaces = handlerType.GetInterfaces();
            bool added = false;
            foreach (var @interface in interfaces) {
                if (@interface.IsGenericType) {
                    var genericTypeDefinition = @interface.GetGenericTypeDefinition();
                    var genericTypeArguments = @interface.GenericTypeArguments;
                    if (typeof(Brimborium.Latrans.Activity.IActivityHandler<,>).Equals(genericTypeDefinition)) {
                        added = true;
                        this.ActivityHandlers.Add(new BuildRequestRelatedType(
                            handlerType.GetTypeInfo(),
                            genericTypeArguments[0].GetTypeInfo(),
                            genericTypeArguments[1].GetTypeInfo()));
                    } else if (typeof(Brimborium.Latrans.Activity.IDispatchActivityHandler<,>).Equals(genericTypeDefinition)) {
                        added = true;
                        this.ActivityHandlers.Add(new BuildRequestRelatedType(
                            handlerType.GetTypeInfo(),
                            genericTypeArguments[0].GetTypeInfo(),
                            genericTypeArguments[1].GetTypeInfo()));
                    }
                }
            }
            if (!added) {
                throw new ArgumentException($"{handlerType.FullName} not supported.", nameof(handlerType));
            }
            //
            return this;
        }

        // 
        public IMediatorBuilder UseStartup<T>() {
            //
            System.Activator.CreateInstance(typeof(T));
            return this;
        }

        public void Build() {
            var hsTypesHandled = new HashSet<Type>();
            var dictDispatchActivityHandlersByRequestType = this.DispatchActivityHandlers.GroupBy(
                    h => new RequestResponseType(
                        h.RequestType,
                        h.ResponseType)
                ).ToDictionary(
                    g => g.Key,
                    g => g.ToList()
                );
            var dictActivityHandlersByRequestType = this.ActivityHandlers.GroupBy(
                    h => new RequestResponseType(
                        h.RequestType,
                        h.ResponseType)
                ).ToDictionary(
                    g => g.Key,
                    g => g.ToList()
                );
            //
            this.DispatchActivityHandlers.Clear();
            this.ActivityHandlers.Clear();
            //
            foreach (var kvD in dictDispatchActivityHandlersByRequestType) {
                var dispatchRequestResponseTypeKey = kvD.Key;
                if (kvD.Value.Count > 1) {
                    throw new InvalidOperationException($"Fatal config: DispatchActivityHandler { dispatchRequestResponseTypeKey} is not unique: { string.Join(", ", kvD.Value.Select(t => t.ActivityHandler.FullName)) }.");
                } else {
                    var dispatchRT = kvD.Value[0];
                    if (!(
                        (dispatchRequestResponseTypeKey.RequestType.Equals(dispatchRT.RequestType))
                        && (dispatchRequestResponseTypeKey.ResponseType.Equals(dispatchRT.ResponseType))
                        )) {
                        throw new InvalidOperationException($"Fatal program: dispatchRequestTypeKey != dispatchRequestType: {dispatchRequestResponseTypeKey} != {dispatchRT}");
                    }

                    if (dictActivityHandlersByRequestType.TryGetValue(dispatchRT.RequestResponseType, out var lstActivityHandlersByRequestType)) {

                        if (hsTypesHandled.Add(dispatchRT.ActivityHandler)) {

                            var (activityContextType, factoryActivityContext, factoryClientConnected)
                                = this.AddServiceActivityContextType(dispatchRT);
                            this.AddServiceDispatchActivityHandlerType(dispatchRT);

                            var lstActivityHandlers = new List<Type>();
                            foreach (var activityHandlerRT in lstActivityHandlersByRequestType) {
                                if (hsTypesHandled.Add(activityHandlerRT.ActivityHandler)) {
                                    var relatedRequestType = activityHandlerRT.RequestType;
                                    if (!dispatchRequestResponseTypeKey.Equals(relatedRequestType)) {
                                        throw new InvalidOperationException($"Fatal program: dispatchRequestTypeKey != relatedRequestType: {dispatchRequestResponseTypeKey} != {relatedRequestType}");
                                    }
                                    //
                                    this.AddServiceActivityHandlerType(activityHandlerRT);
                                    hsTypesHandled.Add(activityHandlerRT.ActivityHandler);
                                    lstActivityHandlers.Add(activityHandlerRT.ActivityHandler);
                                }
                            }

                            this._RequestRelatedTypes.Add(
                                new RequestRelatedType(
                                    dispatchRT.RequestType,
                                    dispatchRT.ResponseType,
                                    dispatchRT.ActivityHandler,
                                    lstActivityHandlers.ToArray(),
                                    activityContextType,
                                    factoryActivityContext,
                                    factoryClientConnected));
                        }
                    }
                }
            }
            //
            foreach (var kvActivityHandlersByRequestType in dictActivityHandlersByRequestType) {
                foreach (var activityHandlerRT in kvActivityHandlersByRequestType.Value) {
                    if (hsTypesHandled.Add(activityHandlerRT.ActivityHandler)) {
                        var (activityContextType, factoryActivityContext, factoryClientConnected)
                                =
                            this.AddServiceActivityContextType(activityHandlerRT);
                        this.AddServiceActivityHandlerType(activityHandlerRT);
                        hsTypesHandled.Add(activityHandlerRT.ActivityHandler);
                        this._RequestRelatedTypes.Add(
                            new RequestRelatedType(
                                activityHandlerRT.RequestType,
                                activityHandlerRT.ResponseType,
                                null,
                                new Type[] { activityHandlerRT.ActivityHandler },
                                activityContextType,
                                factoryActivityContext,
                                factoryClientConnected));
                    }
                }
            }
        }

        private (
            Type activityContextType,
            ObjectFactory factoryActivityContext,
            ObjectFactory factoryClientConnected
            ) AddServiceActivityContextType(BuildRequestRelatedType rt) {
            var activityContextType = typeof(Brimborium.Latrans.Mediator.MediatorContext<,>).MakeGenericType(rt.RequestType, rt.ResponseType);

            var factoryActivityContext = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateFactory(
                activityContextType,
                new Type[] { typeof(CreateActivityContextArguments), rt.RequestType });

            var medaitorClientConnectedType = typeof(Brimborium.Latrans.Mediator.MediatorClientConnected<,>).MakeGenericType(rt.RequestType, rt.ResponseType);

            var factoryClientConnected = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateFactory(
                medaitorClientConnectedType,
                new Type[] { typeof(CreateClientConnectedArguments), rt.RequestType });

            this._Services.AddTransient(activityContextType, activityContextType);
            return (activityContextType, factoryActivityContext, factoryClientConnected);
        }

        private void AddServiceDispatchActivityHandlerType(BuildRequestRelatedType rt) {
            var activityHandler1Type = typeof(Brimborium.Latrans.Activity.IDispatchActivityHandler<>).MakeGenericType(rt.RequestType);
            var activityHandler2Type = typeof(Brimborium.Latrans.Activity.IDispatchActivityHandler<,>).MakeGenericType(rt.RequestType, rt.ResponseType);
            this._Services.AddTransient(activityHandler1Type, rt.ActivityHandler);
            this._Services.AddTransient(activityHandler2Type, rt.ActivityHandler);
            this._Services.AddTransient(rt.ActivityHandler, rt.ActivityHandler);
        }

        private void AddServiceActivityHandlerType(BuildRequestRelatedType rt) {
            var activityHandler1Type = typeof(Brimborium.Latrans.Activity.IActivityHandler<>).MakeGenericType(rt.RequestType);
            var activityHandler2Type = typeof(Brimborium.Latrans.Activity.IActivityHandler<,>).MakeGenericType(rt.RequestType, rt.ResponseType);
            this._Services.AddTransient(activityHandler1Type, rt.ActivityHandler);
            this._Services.AddTransient(activityHandler2Type, rt.ActivityHandler);
            this._Services.AddTransient(rt.ActivityHandler, rt.ActivityHandler);
        }

#if false
        public MediatorBuilder AddHandler2<THandler>() {
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
                                    null,
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
#endif
    }

    public struct RequestResponseType {
        public RequestResponseType(
                TypeInfo requestType,
                TypeInfo responseType
            ) {
            this.RequestType = requestType;
            this.ResponseType = responseType;
        }

        public TypeInfo RequestType { get; }
        public TypeInfo ResponseType { get; }

        public override string ToString() {
            return $"{this.RequestType.FullName}-{this.ResponseType.FullName}";
        }
    }

    public struct BuildRequestRelatedType {
        public BuildRequestRelatedType(
                TypeInfo activityHandler,
                TypeInfo requestType,
                TypeInfo responseType
            ) {
            this.ActivityHandler = activityHandler;
            this.RequestResponseType = new RequestResponseType(requestType, responseType);
        }
        public BuildRequestRelatedType(
                TypeInfo activityHandler,
                RequestResponseType requestResponseType
            ) {
            this.ActivityHandler = activityHandler;
            this.RequestResponseType = requestResponseType;
        }
        public RequestResponseType RequestResponseType { get; }
        public TypeInfo ActivityHandler { get; }
        public TypeInfo RequestType => this.RequestResponseType.RequestType;
        public TypeInfo ResponseType => this.RequestResponseType.ResponseType;

        public override string ToString() {
            return $"{this.RequestType.FullName}-{this.ResponseType.FullName}--{this.ActivityHandler.FullName}";
        }
    }
}
