﻿using Brimborium.Latrans.Activity;

using System;
using System.Collections.Generic;
using System.Runtime;

namespace Brimborium.Latrans.Mediator {
    public sealed class RequestRelatedTypes {
        public readonly Dictionary<Type, RequestRelatedType> Items;
        public RequestRelatedTypes() {
            this.Items = new Dictionary<Type, RequestRelatedType>();
        }
        public RequestRelatedTypes(Dictionary<Type, RequestRelatedType> items) {
            this.Items = new Dictionary<Type, RequestRelatedType>(items);
        }

        public bool TryGetValue(Type requestType, out RequestRelatedType requestRelatedType)
            => this.Items.TryGetValue(requestType, out requestRelatedType);

        public void Add(RequestRelatedType requestRelatedType) {
            this.Items.Add(requestRelatedType.RequestType, requestRelatedType);
        }
    }
    public class CreateActivityContextArguments {
        //public IServiceProvider ServiceProvider;
        public IMediatorService MedaitorService;
    }

    public class CreateClientConnectedArguments {
        //public IServiceProvider ServiceProvider;
        public IMediatorService MedaitorService;
        public RequestRelatedType RequestRelatedType;
    }

    public sealed class RequestRelatedType: IRequestRelatedType {
        public RequestRelatedType() {
        }

        public RequestRelatedType(
            Type requestType,
            Type responseType,
            Type handlerType,
            Type activityContextType,
            Func<CreateActivityContextArguments, object, IActivityContext> createActivityContext,
            Func<CreateClientConnectedArguments, object, IMediatorClientConnected> createClientConnected
            ) {
            this.RequestType = requestType;
            this.ResponseType = responseType;
            this.HandlerTypes = new Type[] { handlerType };
            this.ActivityContextType = activityContextType;
            this.CreateActivityContext = createActivityContext;
            this.CreateClientConnected = createClientConnected;
        }

        public Func<CreateActivityContextArguments, object, IActivityContext> CreateActivityContext { get; set; }
        public Func<CreateClientConnectedArguments, object, IMediatorClientConnected> CreateClientConnected { get; set; }
        
        public Type RequestType { get; set; }
        public Type ResponseType { get; set; }
        public Type DispatcherType { get; set; }
        public Type[] HandlerTypes { get; set; }
        public Type ActivityContextType { get; set; }

        public void AddHandlerType(Type handlerType) {
            Type[] old = this.HandlerTypes;
            var next = new Type[old.Length + 1];
            old.CopyTo(next, 0);
            next[old.Length] = handlerType;
        }
    }
}
