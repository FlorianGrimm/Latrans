using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Activity {
    public class ActivityHandlerBase<TRequest, TResponse>
        : IActivityHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IRequestBase
        where TResponse : IResponseBase {
        protected ActivityHandlerBase() {
        }

        /*
        Task IActivityHandler.SendAsync(
            IActivityContext activityContext,
            CancellationToken cancellationToken) {
            var activityContext2 = (IActivityContext<TRequest, TResponse>)activityContext;
            return this.ExecuteAsync(activityContext2, cancellationToken);
        }
        */

        public virtual Task ExecuteAsync(
            IActivityContext<TRequest, TResponse> activityContext,
            CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }

    }
}
