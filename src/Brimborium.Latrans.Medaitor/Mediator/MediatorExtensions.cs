using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Mediator;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Microsoft.Extensions.DependencyInjection {
}
namespace Brimborium.Latrans.Mediator {
    public static class MediatorExtensions {
        public static async Task ExecuteAsync(
            this IMediatorClient medaitorClient,
            IActivityContext medaitorContext,
            ActivityWaitForSpecification waitForSpecification,
            CancellationToken cancellationToken
            ) {
            await medaitorClient.SendAsync(medaitorContext, cancellationToken);
            if (cancellationToken.IsCancellationRequested) {
                return;
            } else {
                await medaitorClient.WaitForAsync(medaitorContext, waitForSpecification, cancellationToken);
            }
        }

        public static IMediatorClientConnected<TRequest, TResponse> AsMediatorClientConnected<TRequest, TResponse>(
            IMediatorClientConnected<TRequest> mediatorClientConnected
            )
            where TRequest : class, IRequest<TResponse>
            where TResponse : class, IResponseBase {
            return (IMediatorClientConnected<TRequest, TResponse>)mediatorClientConnected;
        }


        //public static async Task<TResponse> XXXAsync<TRequest, TResponse>(
        //    this IMediatorClient medaitorClient,
        //    TRequest request
        //    )
        //    where TRequest: class, IRequest<TResponse>
        //    where TResponse : class, IResponseBase {
        //    await Task.CompletedTask;
        //    return default;
        //}
        //public static async Task<TResponse> XXX2Async<TRequest, TResponse>(
        //    this IMediatorClient medaitorClient,
        //    TRequest request,
        //    ActivityWaitForSpecification waitForSpecification,
        //    CancellationToken cancellationToken
        //    )
        //    where TRequest : class, IRequest<TResponse>
        //    where TResponse : class, IResponseBase {
        //    await Task.CompletedTask;
        //    return default;
        //}

        public static bool TryGetResult<T>(
                this IActivityResponse response,
                out T result
            ) {
            if (response is OkResultActivityResponse<T> okResult) {
                result = okResult.Result;
                return true;
            } else {
                result = default;
                return false;
            }
        }

        public static T GetResultOrDefault<T>(
                this IActivityResponse response,
                T defaultValue = default
            ) {
            if (response is OkResultActivityResponse<T> okResult) {
                return okResult.Result;
            } else {
                return defaultValue;
            }
        }
    }

#if false
    public class RequestResponsePair<TRequest, TResponse>
        : IDisposable
        where TRequest : class, IRequest<TResponse>, IRequestBase
        where TResponse : IResponseBase {
        private int _IsDisposed;

        public RequestResponsePair() {
        }

        public async Task<object> ExecuteAsync(
            TRequest request,
            IMediatorAccess medaitorAccess,
            ActivityWaitForSpecification waitForSpecification,
            CancellationToken cancellationToken
            ) {
            using var client = medaitorAccess.GetMedaitorClient();
            using var connected = await client.ConnectAsync(request);
            var response = await connected.WaitForAsync(null, cancellationToken);
            return null;
        }
        protected virtual void Dispose(bool disposing) {
            if (0 == System.Threading.Interlocked.Exchange(ref this._IsDisposed, 1)) {
                if (disposing) {
                } else { 
                }
            }
        }

        ~RequestResponsePair() {
            Dispose(disposing: false);
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
#endif
}
