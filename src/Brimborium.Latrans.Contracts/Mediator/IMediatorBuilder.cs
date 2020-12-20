
using Brimborium.Latrans.Activity;

using System;

namespace Brimborium.Latrans.Mediator {
    // 
    public interface IMediatorBuilder {
        IMediatorBuilder AddActivityHandler<THandler>()
            where THandler : IActivityHandler;
        IMediatorBuilder AddDispatchHandler<THandler>()
            where THandler : IDispatchActivityHandler;
        IMediatorBuilder AddHandlerType(Type handlerType);
        IMediatorBuilder UseStartup<T>();
        void Build();
    }

    //public interface IRequestRelatedType {
    //    public Type DispatcherType { get; set; }
    //    public Type[] HandlerTypes { get; set; }
    //}
}
