using Brimborium.Latrans.Activity;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {
    public class RequestResponseHelper<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IRequestBase
        where TResponse : IResponseBase {

#if false
        public static async Task<ActionResult<TResult>> ExecuteToActionResultAsync<TResult>(
            IMediatorClientFactory medaitorAccess,
            TRequest request,
            Func<TResponse, TResult> extractResult,
            ActivityWaitForSpecification waitForSpecification,
            System.Threading.CancellationToken requestAborted) {
            try { 
                using var client = medaitorAccess.GetMedaitorClient();
                using var connected = await client.ConnectAsync(request, requestAborted);
                var response = await connected.WaitForAsync(waitForSpecification, requestAborted);
                return response.ConvertResponseToActionResult<TResponse, TResult>(extractResult);
            } catch (System.Exception error) {
                return new ObjectResult(error.Message) {
                    StatusCode = 500
                };
            }
        }
#endif
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
