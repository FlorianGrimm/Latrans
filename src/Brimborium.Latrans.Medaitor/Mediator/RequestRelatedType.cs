using Brimborium.Latrans.Activity;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime;
using System.Threading;

namespace Brimborium.Latrans.Mediator {
    public sealed class RequestRelatedTypes {
        public readonly Dictionary<Type, RequestRelatedType> Items;
        public RequestRelatedTypes() {
            this.Items = new Dictionary<Type, RequestRelatedType>();
        }
        public RequestRelatedTypes(Dictionary<Type, RequestRelatedType> items) {
            this.Items = new Dictionary<Type, RequestRelatedType>(items);
        }

        public bool TryGetValue(Type requestType, [MaybeNullWhen(false)] out RequestRelatedType requestRelatedType)
            => this.Items.TryGetValue(requestType, out requestRelatedType);

        public void Add(RequestRelatedType requestRelatedType) {
            this.Items.Add(requestRelatedType.RequestType, requestRelatedType);
        }
    }
    public class CreateActivityContextArguments {
        public readonly IMediatorService MedaitorService;
        public readonly MediatorScopeService MediatorScopeService;
        public readonly ActivityId ActivityId;

        public CreateActivityContextArguments(
                IMediatorService medaitorService, 
                MediatorScopeService mediatorScopeService,
                ActivityId activityId
            ) {
            this.MedaitorService = medaitorService;
            this.MediatorScopeService = mediatorScopeService;
            this.ActivityId = activityId;
        }
    }

    public class CreateClientConnectedArguments {
        public CreateClientConnectedArguments(
                IMediatorServiceInternalUse medaitorService,
                IMediatorScopeServiceInternalUse mediatorScopeService,
                ActivityId activityId,
                RequestRelatedType requestRelatedType
            ) {
            this.MedaitorService = medaitorService;
            this.MediatorScopeService = mediatorScopeService;
            this.ActivityId = activityId;
            this.RequestRelatedType = requestRelatedType;
        }
        public CreateClientConnectedArguments(
                IMediatorServiceInternalUse medaitorService,
                IMediatorClient medaitorClient,
                ActivityId activityId,
                RequestRelatedType requestRelatedType
            ) {
            this.MedaitorService = medaitorService;
            this.MedaitorClient = medaitorClient;
            this.ActivityId = activityId;
            this.RequestRelatedType = requestRelatedType;

        }
        public readonly IMediatorServiceInternalUse MedaitorService;
        public readonly IMediatorClient? MedaitorClient;
        public readonly IMediatorScopeServiceInternalUse? MediatorScopeService;
        public readonly ActivityId ActivityId;
        public readonly RequestRelatedType RequestRelatedType;
    }

    public sealed class RequestRelatedType {
        public RequestRelatedType(
            Type requestType,
            Type responseType,
            Type? dispatcherType,
            Type[] handlerTypes,
            Type activityContextType,
            ObjectFactory factoryActivityContext,
            ObjectFactory factoryClientConnected) {
            this.RequestType = requestType;
            this.ResponseType = responseType;
            this.DispatcherType = dispatcherType;
            this.HandlerTypes = handlerTypes;
            this.ActivityContextType = activityContextType;
            this.FactoryActivityContext = factoryActivityContext;
            this.FactoryClientConnected = factoryClientConnected;
        }

        public ObjectFactory FactoryActivityContext { get; set; }
        public ObjectFactory FactoryClientConnected { get; set; }

        public Type RequestType { get; set; }
        public Type ResponseType { get; set; }
        public Type? DispatcherType { get; set; }
        public Type[] HandlerTypes { get; set; }
        public Type ActivityContextType { get; set; }

#if false
        public void AddHandlerType(Type handlerType) {
            Type[] old = this.HandlerTypes;
            var next = new Type[old.Length + 1];
            old.CopyTo(next, 0);
            next[old.Length] = handlerType;
        }
#endif
    }
}
