using Brimborium.Latrans.Activity;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {
    public interface IMediatorService : IDisposable {
        IMediatorServiceStorage Storage { get; }

        Task<IMediatorClientConnected<TRequest>> ConnectAsync<TRequest>(
            IMediatorClient medaitorClient,
            ActivityId activityId,
            TRequest request,
            ActivityExecutionConfiguration activityExecutionConfiguration,
            CancellationToken cancellationToken
            );
    }

    public interface IMediatorScopeService : IDisposable {
        IMediatorServiceStorage Storage { get; }

        IServiceProvider ServiceProvider { get; }

        Task<IMediatorClientConnected<TRequest>> ConnectAsync<TRequest>(
            ActivityId activityId,
            TRequest request,
            ActivityExecutionConfiguration activityExecutionConfiguration,
            CancellationToken cancellationToken);

        bool AddClientConnected<TRequest>(
            IMediatorClientConnected<TRequest> mediatorClientConnected
            );

        bool RemoveClientConnected<TRequest>(
            IMediatorClientConnected<TRequest> mediatorClientConnected
            );
    }

    public interface IStartupMediator {
        void ConfigureMediatorServices(IMediatorBuilder builder);
    }
}
