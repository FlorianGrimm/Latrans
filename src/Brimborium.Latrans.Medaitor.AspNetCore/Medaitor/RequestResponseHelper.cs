using Brimborium.Latrans.Activity;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {
    public class RequestResponseHelper<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IRequestBase
        where TResponse : IResponseBase {

        public static async Task<ActionResult<TResult>> ExecuteToActionResultAsync<TResult>(
            IMediatorClient client,
            ActivityId activityId, 
            TRequest request,
            Func<TResponse, TResult> extractResult,
            ActivityExecutionConfiguration activityExecutionConfiguration,
            System.Threading.CancellationToken requestAborted) {
            try {
                using var connected = await client.ConnectAndSendAsync(
                    activityId,
                    request,
                    activityExecutionConfiguration,
                    requestAborted);
                var response = await connected.WaitForAsync(activityExecutionConfiguration, requestAborted);
                return response.ConvertResponseToActionResult<TResponse, TResult>(extractResult);
            } catch (System.Exception error) {
                return new ObjectResult(error.Message) {
                    StatusCode = 500
                };
            }
        }
    }
}
