using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Utility {
    public sealed class ActivityCompletion<TResult> : IDisposable {
        private TaskCompletionSource<TResult> _Tcs;
        private int _State;

        public ActivityCompletion() {
            this._Tcs = new TaskCompletionSource<TResult>();
            this._State = 0;
        }

        public Task<TResult> Task => this._Tcs.Task;

        public bool IsNotDefined => (this._State == 0);

        public bool TrySetResult(TResult result) {
            if (0 == System.Threading.Interlocked.CompareExchange(ref this._State, 1, 0)) {
                return this._Tcs.TrySetResult(result);
            } else {
                return false;
            }
        }

        public bool TrySetException(Exception exception) {
            if (exception is null) {
                throw new ArgumentNullException(nameof(exception));
            }
            if (0 == System.Threading.Interlocked.CompareExchange(ref this._State, 1, 0)) {
                return this._Tcs.TrySetException(exception);
            } else {
                return false;
            }
        }

        public bool TrySetCanceled(CancellationToken cancellationToken) {
            if (0 == System.Threading.Interlocked.CompareExchange(ref this._State, 1, 0)) {
                return this._Tcs.TrySetCanceled(cancellationToken);
            } else {
                return false;
            }
        }

        private void Dispose(bool disposing) {
            var state = System.Threading.Interlocked.Exchange(ref this._State, -1);
            if (state == 0) {
                this.TrySetCanceled(CancellationToken.None);
            }
        }

        ~ActivityCompletion() {
            this.Dispose(disposing: false);
        }

        public void Dispose() {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
