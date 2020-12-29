using Brimborium.Latrans.Activity;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Brimborium.Latrans.Mediator {
    public class MediatorBuilder : IMediatorBuilder {
        public IServiceCollection Services { get; }
        public readonly RequestRelatedTypes RequestRelatedTypes;
        public readonly List<ReqResHandler> ActivityHandlers;
        public readonly List<ReqResHandler> DispatchActivityHandlers;

        public MediatorBuilder() {
            this.Services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            this.RequestRelatedTypes = new RequestRelatedTypes();
            this.Services.AddOptions();
            this.Services.AddOptions<ActivityExecutionConfigurationOptions>();
            this.Services.AddTransient<ActivityExecutionConfigurationDefaults>();
            this.ActivityHandlers = new List<ReqResHandler>();
            this.DispatchActivityHandlers = new List<ReqResHandler>();
        }

        //public void ConfigureActivityWaitForSpecificationOptions(
        //        Action<ActivityWaitForSpecificationOptions> configureOptions
        //    ) {
        //    this.Services.Configure<ActivityWaitForSpecificationOptions>(configureOptions);
        //}

        public MediatorOptions GetOptions() {
            this.Services.AddSingleton<ActivityExecutionConfigurationDefaults>();

            return new MediatorOptions(
                this.Services,
                this.RequestRelatedTypes
                );
        }

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
                        this.ActivityHandlers.Add(new ReqResHandler(
                            handlerType.GetTypeInfo(),
                            genericTypeArguments[0].GetTypeInfo(),
                            genericTypeArguments[1].GetTypeInfo()));
                    } else if (typeof(Brimborium.Latrans.Activity.IDispatchActivityHandler<,>).Equals(genericTypeDefinition)) {
                        added = true;
                        this.ActivityHandlers.Add(new ReqResHandler(
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

        public IMediatorBuilder UseConfigure<T>(T? instance = default)
            where T : class, IStartupMediator {
            //
            if (instance is null) {
                instance = (T?)System.Activator.CreateInstance(typeof(T));
            }
            if (instance is null) {
                throw new ArgumentException("instance is null", nameof(instance));
            } else {
                instance.ConfigureMediatorServices(this);
            }
            return this;
        }

        public void Build() {
            var hsTypesHandled = new HashSet<Type>();
            var dictDispatchActivityHandlersByRequestType = this.DispatchActivityHandlers.GroupBy(
                    h => new ReqRes(
                        h.RequestType,
                        h.ResponseType)
                ).ToDictionary(
                    g => g.Key,
                    g => g.ToList()
                );
            var dictActivityHandlersByRequestType = this.ActivityHandlers.GroupBy(
                    h => new ReqRes(
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

                    if (dictActivityHandlersByRequestType.TryGetValue(dispatchRT.ReqRes, out var lstActivityHandlersByRequestType)) {

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

                            this.RequestRelatedTypes.Add(
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
                        this.RequestRelatedTypes.Add(
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
            ) AddServiceActivityContextType(ReqResHandler rt) {
            var activityContextType = typeof(Brimborium.Latrans.Mediator.MediatorContext<>).MakeGenericType(rt.RequestType);

            var factoryActivityContext = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateFactory(
                activityContextType,
                new Type[] { typeof(CreateActivityContextArguments), rt.RequestType });

            var medaitorClientConnectedType = typeof(Brimborium.Latrans.Mediator.MediatorClientConnected<,>).MakeGenericType(rt.RequestType, rt.ResponseType);

            var factoryClientConnected = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateFactory(
                medaitorClientConnectedType,
                new Type[] { typeof(CreateClientConnectedArguments), rt.RequestType });

            this.Services.AddTransient(activityContextType, activityContextType);
            return (activityContextType, factoryActivityContext, factoryClientConnected);
        }

        private void AddServiceDispatchActivityHandlerType(ReqResHandler rt) {
            var activityHandler1Type = typeof(Brimborium.Latrans.Activity.IDispatchActivityHandler<>).MakeGenericType(rt.RequestType);
            var activityHandler2Type = typeof(Brimborium.Latrans.Activity.IDispatchActivityHandler<,>).MakeGenericType(rt.RequestType, rt.ResponseType);
            this.Services.AddTransient(activityHandler1Type, rt.ActivityHandler);
            this.Services.AddTransient(activityHandler2Type, rt.ActivityHandler);
            this.Services.AddTransient(rt.ActivityHandler, rt.ActivityHandler);
        }

        private void AddServiceActivityHandlerType(ReqResHandler rt) {
            var activityHandler1Type = typeof(Brimborium.Latrans.Activity.IActivityHandler<>).MakeGenericType(rt.RequestType);
            var activityHandler2Type = typeof(Brimborium.Latrans.Activity.IActivityHandler<,>).MakeGenericType(rt.RequestType, rt.ResponseType);
            this.Services.AddTransient(activityHandler1Type, rt.ActivityHandler);
            this.Services.AddTransient(activityHandler2Type, rt.ActivityHandler);
            this.Services.AddTransient(rt.ActivityHandler, rt.ActivityHandler);
        }

        public struct ReqRes {
            public ReqRes(
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

        public struct ReqResHandler {
            public ReqResHandler(
                    TypeInfo activityHandler,
                    TypeInfo requestType,
                    TypeInfo responseType
                ) {
                this.ActivityHandler = activityHandler;
                this.ReqRes = new ReqRes(requestType, responseType);
            }
            public ReqResHandler(
                    TypeInfo activityHandler,
                    ReqRes requestResponseType
                ) {
                this.ActivityHandler = activityHandler;
                this.ReqRes = requestResponseType;
            }
            public ReqRes ReqRes { get; }
            public TypeInfo ActivityHandler { get; }
            public TypeInfo RequestType => this.ReqRes.RequestType;
            public TypeInfo ResponseType => this.ReqRes.ResponseType;

            public override string ToString() {
                return $"{this.RequestType.FullName}-{this.ResponseType.FullName}--{this.ActivityHandler.FullName}";
            }
        }
    }
}
