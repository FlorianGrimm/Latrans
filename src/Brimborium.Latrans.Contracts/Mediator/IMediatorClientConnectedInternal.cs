using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {
    public interface IMediatorClientConnectedInternal<TRequest> : IMediatorClientConnected<TRequest> {
        void Initialize();

        Task<IMediatorClientConnected<TRequest>> SendAsync(
                CancellationToken cancellationToken
            );
    }
}
