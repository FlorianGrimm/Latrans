
using Brimborium.Latrans.Activity;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {
    public interface IMediatorClientConnected : IDisposable {
        IActivityContext? GetActivityContext();
        void Initialize();
        Task SendAsync(CancellationToken cancellationToken);
    }

    public interface IMediatorClientConnected<TRequest> : IMediatorClientConnected {
        Task<IActivityResponse> WaitForAsync(
           ActivityExecutionConfiguration waitForSpecification,
           CancellationToken cancellationToken
           );
    }
    public interface IMediatorClientConnected<TRequest, TResponse> : IMediatorClientConnected<TRequest> {
    }
}
