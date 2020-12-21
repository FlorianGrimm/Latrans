using Brimborium.Latrans.Activity;

using Microsoft.Extensions.DependencyInjection;

using System;

namespace Brimborium.Latrans.Mediator {
    // 
    public interface IMediatorBuilder {
        IServiceCollection Services { get; }

        IMediatorBuilder AddActivityHandler<THandler>()
            where THandler : IActivityHandler;
        
        IMediatorBuilder AddDispatchHandler<THandler>()
            where THandler : IDispatchActivityHandler;
        
        IMediatorBuilder AddHandlerType(Type handlerType);

        IMediatorBuilder UseConfigure<T>(T? instance = default)
            where T : class, IStartupMediator;

        void Build();
    }
}
