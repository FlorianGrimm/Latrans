using Brimborium.Latrans.Activity;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Medaitor {
    public class MedaitorClientConnected<TRequest, TResponse>
        : IMedaitorClientConnected<TRequest, TResponse>
        , IMedaitorClientConnectedInternal<TRequest>
        , IMedaitorClientConnected<TRequest>
        , IMedaitorClientConnected
        , IDisposable {

        /// <summary>
        /// Internal use.
        /// Used in <see cref="MediatorBuilder.AddHandler{THandler}"/>.
        /// </summary>
        /// <returns>Function that creates the context.</returns>
        public static Func<CreateClientConnectedArguments, object, IMedaitorClientConnected> GetCreateInstance()
            => ((CreateClientConnectedArguments arguments, object request) => new MedaitorClientConnected<TRequest, TResponse>(arguments, (TRequest)request));

        private int _IsDisposed;

        public MedaitorClientConnected() {
        }

        public MedaitorClientConnected(CreateClientConnectedArguments arguments, TRequest request) {

        }

        public Task<IMedaitorClientConnected<TRequest>> SendAsync() {
            throw new NotImplementedException();
        }

        public Task<IActivityResponse> WaitForAsync(IActivityContext activityContext, ActivityWaitForSpecification waitForSpecification, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        ~MedaitorClientConnected() {
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
