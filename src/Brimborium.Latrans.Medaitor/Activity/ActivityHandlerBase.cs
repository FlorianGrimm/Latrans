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
    public static class IActivityHandlerExtension {
        public static Task SetResponseAsync<TRequest, TResponse>(
            this IActivityHandler<TRequest, TResponse> that,
            IActivityContext<TRequest> activityContext,
            TResponse response)
            where TRequest : IRequest<TResponse>, IRequestBase
            where TResponse : IResponseBase
            {
            var okResponse = new OkResultActivityResponse<TResponse>(response);
            return activityContext.SetActivityResponseAsync(okResponse);
        }
    }
}
