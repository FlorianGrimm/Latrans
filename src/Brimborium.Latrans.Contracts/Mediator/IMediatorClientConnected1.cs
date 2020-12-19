using Brimborium.Latrans.Activity;

using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {
    public interface IMediatorClientConnected<TRequest> : IMediatorClientConnected {
        Task<IActivityResponse> WaitForAsync(
           ActivityWaitForSpecification waitForSpecification,
           CancellationToken cancellationToken
           );
    }

    //public interface IRequestRelatedType {
    //    public Type DispatcherType { get; set; }
    //    public Type[] HandlerTypes { get; set; }
    //}
}
