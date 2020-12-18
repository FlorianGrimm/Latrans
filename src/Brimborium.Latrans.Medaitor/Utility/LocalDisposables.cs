using System;
using System.Collections.Immutable;

namespace Brimborium.Latrans.Utility {
    public class LocalDisposables : ILocalDisposables {
        public static LocalDisposables Create()
            => new LocalDisposables();

        private AtomicReference<ImmutableList<IDisposable>> _Disposables;

        public LocalDisposables() {
            this._Disposables = new AtomicReference<ImmutableList<IDisposable>>(ImmutableList<IDisposable>.Empty);
        }

        public T Add<T>(T value)
            where T : class, IDisposable {
            this._Disposables.Mutate1<T>(
                value,
                (item, disposables) => disposables.Add(item)
                );
            return value;
        }

        public void Dispose() {
            var oldDisposables = this._Disposables.Mutate((disposables) => ImmutableList<IDisposable>.Empty);
             
            System.Exception error = null;
            foreach (var disposable in oldDisposables) {
                if (disposable is object) {
                    try {
                        disposable.Dispose();
                    } catch (System.Exception e) {
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
