using Brimborium.Latrans.Activity;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {
    public class MediatorClientConnected<TRequest, TResponse>
        : IMediatorClientConnected<TRequest, TResponse>
        , IMediatorClientConnectedInternal<TRequest>
        , IMediatorClientConnected<TRequest>
        , IMediatorClientConnected
        , IDisposable {

        /// <summary>
        /// Internal use.
        /// Used in <see cref="MediatorBuilder.AddHandler{THandler}"/>.
        /// </summary>
        /// <returns>Function that creates the context.</returns>
        public static Func<CreateClientConnectedArguments, object, IMediatorClientConnected> GetCreateInstance()
            => ((CreateClientConnectedArguments arguments, object request) => new MediatorClientConnected<TRequest, TResponse>(arguments, (TRequest)request));

        private int _IsDisposed;

        public MediatorClientConnected() {
        }

        public MediatorClientConnected(CreateClientConnectedArguments arguments, TRequest request) {

        }

        public Task<IMediatorClientConnected<TRequest>> SendAsync() {
            throw new NotImplementedException();
        }

        public Task<IActivityResponse> WaitForAsync(
            ActivityWaitForSpecification waitForSpecification,
            CancellationToken cancellationToken
            ) {
            throw new NotImplementedException();
        }

        ~MediatorClientConnected() {
            this.Dispose(disposing: false);
        }

        public void Dispose() {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (0 == System.Threading.Interlocked.Exchange(ref this._IsDisposed, 1)) {
                //if (disposing) {
                //    if (_ActivityCompletion.IsNotDefined) {
                //        _ActivityCompletion.TrySetResult(new FailureActivityResponse(new Exception("Disposing")));
                //    }
                //} else {
                //    if (_ActivityCompletion.IsNotDefined) {
                //        _ActivityCompletion.TrySetResult(new FailureActivityResponse(new Exception("Disposed")));
                //    }
                //}
                //_ActivityCompletion.Dispose();
            }
        }
    }
}
