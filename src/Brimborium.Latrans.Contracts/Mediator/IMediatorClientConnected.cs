using Brimborium.Latrans.Activity;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {
    public interface IMediatorClientConnected : IDisposable {
        IActivityContext? GetActivityContext();
        void Initialize();
        Task SendAsync(CancellationToken cancellationToken);
        Task<MediatorActivityStatus> GetStatusAsync();
        Task<IActivityResponse> WaitForAsync(
           ActivityExecutionConfiguration waitForSpecification,
           CancellationToken cancellationToken
           );
    }

    public interface IMediatorClientConnected<TRequest> : IMediatorClientConnected {
    }

    public interface IMediatorClientConnected<TRequest, TResponse> : IMediatorClientConnected<TRequest> {
    }
}
