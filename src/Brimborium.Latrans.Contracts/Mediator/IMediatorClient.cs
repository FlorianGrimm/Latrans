using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {

    public interface IMediatorClient : IDisposable {
        bool IsDisposed { get; }

        Task<IMediatorClientConnected<TRequest>> ConnectAsync<TRequest>(
            TRequest request,
            CancellationToken cancellationToken
            );
#if false
        IActivityContext<TRequest> CreateContextByRequest<TRequest>(
            TRequest request
            );

        Task SendAsync(
            IActivityContext activityContext,
            CancellationToken cancellationToken
            );

        Task WaitForAsync(
            IActivityContext activityContext,
            ActivityWaitForSpecification waitForSpecification,
            CancellationToken cancellationToken
            );
#endif
    }

    //public interface IRequestRelatedType {
    //    public Type DispatcherType { get; set; }
    //    public Type[] HandlerTypes { get; set; }
    //}
}
