using Brimborium.Latrans.Activity;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {
    public interface IMediatorService : IDisposable {
        IMediatorServiceStorage Storage { get; }

        Task<IMediatorClientConnected<TRequest>> ConnectAsync<TRequest>(
            IMediatorClient medaitorClient,
            TRequest request,
            CancellationToken cancellationToken);
    }

    public interface IMediatorServiceInternalUse { 
        DateTime GetUtcNow();
    }

    public interface IMediatorScopeService : IDisposable {
        IMediatorServiceStorage Storage { get; }

        IServiceProvider ServiceProvider { get; }

        void AddClientConnected<TRequest>(
            IMediatorClientConnected<TRequest> mediatorClientConnected
            );
    }

    public interface IStartupMediator {
        void ConfigureMediatorServices(IMediatorBuilder builder);
    }
}
