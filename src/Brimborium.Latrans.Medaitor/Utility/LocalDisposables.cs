using System;
using System.Collections.Immutable;

namespace Brimborium.Latrans.Utility {

    public class OwnedDisposable : IOwnedDisposable, IDisposable {
        private readonly object _Owner;
        private int _IsDisposed;

        protected OwnedDisposable() {
            this._Owner = this;
        }

        protected OwnedDisposable(object owner) {
            this._Owner = owner;

        }

        public void Dispose(object owner) {
            if (ReferenceEquals(this._Owner, owner)) {
                this.Dispose();
            }
        }

        private void DisposeInstance(bool disposing) {
            if (0 == System.Threading.Interlocked.CompareExchange(ref this._IsDisposed, 1, 0)) {
                if (disposing) {
                    this.Dispose();
                } else {
                    try { this.Dispose(); } catch { }
                }
            }
        }

        protected virtual void Dispose(bool disposing) {
        }

        ~OwnedDisposable() {
            this.DisposeInstance(disposing: false);
        }

        public void Dispose() {
            this.DisposeInstance(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
    public class LocalDisposables : ILocalDisposables {
        public static LocalDisposables Create(object owner)
            => new LocalDisposables(owner);

        private AtomicReference<ImmutableList<IDisposable>> _Disposables;

        private readonly object _Owner;

        private LocalDisposables(object owner) {
            this._Disposables = new AtomicReference<ImmutableList<IDisposable>>(ImmutableList<IDisposable>.Empty);
            this._Owner = owner;
        }

        public LocalDisposables() {
            this._Disposables = new AtomicReference<ImmutableList<IDisposable>>(ImmutableList<IDisposable>.Empty);
            this._Owner = this;
        }

        public T Add<T>(T value)
            where T : class, IDisposable {
            this._Disposables.Mutate1<T>(
                value,
                (item, disposables) => disposables.Add(item)
                );
            return value;
        }

        public void Dispose(object owner) {
            if (ReferenceEquals(owner, this._Owner)) {
                this.Dispose();
            }
        }

        public void Dispose() {
            var oldDisposables = this._Disposables.Mutate((disposables) => ImmutableList<IDisposable>.Empty);

            System.Exception? error = null;
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
