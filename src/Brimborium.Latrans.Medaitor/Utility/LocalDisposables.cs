using System;

namespace Brimborium.Latrans.Utility {
    public class LocalDisposables : ILocalDisposables {
        public static LocalDisposables Create()
            => new LocalDisposables();

        private IDisposable[] _Disposables;

        public LocalDisposables() {
            this._Disposables = Array.Empty<IDisposable>();
        }

        public T Add<T>(T value) 
            where T : class, IDisposable {
            while (true) {
                var current = this._Disposables;
                var next = new IDisposable[current.Length + 1];
                if (current.Length > 0) {
                    current.CopyTo(next, 0);
                }
                next[current.Length] = value;
                if (ReferenceEquals(
                        System.Threading.Interlocked.CompareExchange(
                            ref this._Disposables,
                            next,
                            current
                            ),
                        current
                    )) {
                    return value;
                }
            }
        }

        public void Dispose() {
            var disposables = System.Threading.Interlocked.Exchange(
                ref this._Disposables, 
                Array.Empty<IDisposable>());
            System.Exception error=null;
            foreach (var disposable in disposables) {
                if (disposable is object) {
                    try {
                        disposable.Dispose();
                    } catch (System.Exception e){
                        error = e;
                    }
                }
            }
            if (error is object) {
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw();
            }
        }
    }

    public static class LocalDisposablesExtension { 
        public static T AddUsingValue<T>(this ILocalDisposables localDisposables, IUsingValue<T> usingValue) 
            where T : class, IDisposable {
            var result = usingValue.GetValue();
            localDisposables.Add(usingValue);
            return result;
        }
    }
}
