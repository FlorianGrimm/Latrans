using System;
using System.Collections.Generic;

namespace Brimborium.Latrans.Medaitor {
    public class RequestRelatedTypes {
        public readonly Dictionary<Type, RequestRelatedType> Items;
        public RequestRelatedTypes() {
            this.Items = new Dictionary<Type, RequestRelatedType>();
        }

        public bool TryGetValue(Type requestType, out RequestRelatedType requestRelatedType)
            => this.Items.TryGetValue(requestType, out requestRelatedType);

        public void Add(RequestRelatedType requestRelatedType) {
            this.Items.Add(requestRelatedType.RequestType, requestRelatedType);
        }
    }
    public class RequestRelatedType {
        public RequestRelatedType(Type requestType, Type responseType, Type handlerType, Type activityContextType) {
            this.RequestType = requestType;
            this.ResponseType = responseType;
            this.HandlerTypes = new Type[] { handlerType };
            this.ActivityContextType = activityContextType;
        }

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
