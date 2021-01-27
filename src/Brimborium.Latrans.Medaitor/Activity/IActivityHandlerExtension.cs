using System.Threading.Tasks;

namespace Brimborium.Latrans.Activity {
    public static class IActivityHandlerExtension {
        public static Task SetResponseAsync<TRequest, TResponse>(
            this IActivityHandler<TRequest, TResponse> that,
            IActivityContext<TRequest> activityContext,
            TResponse response)
            where TRequest : IRequest<TResponse>, IRequestBase
            where TResponse : IResponseBase {
            var okResponse = new OkResultActivityResponse<TResponse>(response);
            return activityContext.SetActivityResponseAsync(okResponse);
        }

        public static Task SetFailureResponseAsync<TRequest, TResponse>(
            this IActivityHandler<TRequest, TResponse> that,
            IActivityContext<TRequest> activityContext,
            System.Exception error)
            where TRequest : IRequest<TResponse>, IRequestBase
            where TResponse : IResponseBase {
            var failureResponse = new FailureActivityResponse(error);
            return activityContext.SetActivityResponseAsync(failureResponse);
        }

    }
}
