
using System;
using System.Runtime.Serialization;

namespace Brimborium.Latrans.Utility {
    public class ActionDisposable<T>
        : IDisposable
        where T : class {
        private int _IsDisposed;
        private T? _Instance;
        private Action<T>? _Action;

        public ActionDisposable(T instance, Action<T> action) {
            this._Instance = instance;
            this._Action = action;
        }

        private void Dispose(bool disposing) {
            if (disposing) {
                Action<T>? a = System.Threading.Interlocked.Exchange(ref this._Action, null);
                if (a is null) {
                    //
                } else {
                    T i = System.Threading.Interlocked.Exchange(ref this._Instance, null)!;
                    a(i);
                }
            } else {
                try { this.Dispose(true); } catch { }
            }
        }

        ~ActionDisposable() {
            this.Dispose(disposing: false);
        }

        public void Dispose() {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
