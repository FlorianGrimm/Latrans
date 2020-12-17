using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Utility;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Brimborium.Latrans.Mediator {
    public static class ActivityContextExtensions {
        // old
        public static ActionResult<T> ReturnAsActionResult<T>(
            this IActivityContext activityContext
            ) {
            if (activityContext is null) {
                throw new ArgumentNullException(nameof(activityContext));
            }

            //activityContext.GetResult();
            throw new NotImplementedException();
        }

        // next
        public static ActionResult<TResult> ConvertResponseToActionResult<TResponse, TResult>(
            this IActivityResponse response,
            Func<TResponse, TResult> extractResult
            ) {
            if (response is null) {
                throw new ArgumentNullException(nameof(response));
            }

            if (response is OkResultActivityResponse<TResponse> okResult) {
                var resultValue = extractResult(okResult.Result);
                return new ActionResult<TResult>(resultValue);
                //return new Microsoft.AspNetCore.Mvc.OkObjectResult();
            }

            //activityContext.GetResult();
            throw new NotImplementedException();
        }
    }
}
