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
    public interface IMediatorScopeService : IDisposable {
        IMediatorServiceStorage Storage { get; }

        Task<IMediatorClientConnected<TRequest>> ConnectAsync<TRequest>(
            IMediatorClient medaitorClient,
            TRequest request,
            CancellationToken cancellationToken);
    }
    public interface IStartupMediator {
        void ConfigureMediatorServices(IMediatorBuilder builder);
    }
}
