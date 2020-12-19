using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {
    public interface IMediatorClientConnectedInternal<TRequest> : IMediatorClientConnected<TRequest> {
        Task<IMediatorClientConnected<TRequest>> SendAsync(
                CancellationToken cancellationToken
            );
    }

    //public interface IRequestRelatedType {
    //    public Type DispatcherType { get; set; }
    //    public Type[] HandlerTypes { get; set; }
    //}
}
