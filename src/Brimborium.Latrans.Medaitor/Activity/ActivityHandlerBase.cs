using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Activity {
    public abstract class ActivityHandlerBase<TRequest, TResponse>
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

        public abstract Task ExecuteAsync(
            IActivityContext<TRequest> activityContext,
            CancellationToken cancellationToken);

    }
}
