using Brimborium.Latrans.Mediator;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Activity {

    public interface IActivityContext 
        : IMediatorClient {

        ActivityStatus Status {get;set;}

        ActivityId ActivityId { get; set; }
        
        //Guid OperationId { get; set; }
        //Guid ExecutionId { get; set; }
        
        Type GetRequestType();

        Task AddActivityEventAsync(IActivityEvent activityEvent);

        Task SetFailureAsync(System.Exception error);

        Task SetActivityResponseAsync(IActivityResponse activityResponse);

        Task<IActivityResponse> GetActivityResponseAsync();

        IMediatorScopeService MediatorScopeService { get; }

        Task<MediatorActivityStatus> GetStatusAsync();
    }

    public interface IActivityContext<TRequest>
        : IActivityContext {
        TRequest Request { get; set; }
    }

#if false
    public interface IActivityContext<TRequest, TResponse>
        : IActivityContext<TRequest>
        , IDisposable {
        Task SetResponseAsync(TResponse response);
    }
#endif

    public class ActivityInvoker<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IRequestBase
        where TResponse : IResponseBase {
        public async Task<TResponse> ExecuteAsync(
            IActivityContext<TRequest> activityContext,
            ActivityId activityId,
            TRequest request,
            ActivityExecutionConfiguration activityExecutionConfiguration,
            CancellationToken cancellationToken) {
            using var connectedClient = await activityContext.ConnectAndSendAsync(
                activityId,
                request,
                activityExecutionConfiguration,
                cancellationToken);

            var response = await connectedClient.WaitForAsync(activityExecutionConfiguration, cancellationToken);
            if (response is IOkResultActivityResponse resultActivityResponse) {
                var success = resultActivityResponse.TryGetResult(out var result);
                if (result is TResponse resultTyped) {
                    return resultTyped;
                } else { 
                }
            }
            //response.GetAsActivityEvent
            //if (response is OkResultActivityResponse<TResponse> okResult) {
            //    if (response.TryGetResult<TResponse>(out var result)) {
            //    return result;
            //} else {
            //}
            throw new NotImplementedException();
        }
    }
}
