
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Runtime.CompilerServices;

namespace Brimborium.Latrans.Utility {
    public static class LazyValue {
        public static ILazyValue<T> CreateByFunction<T>(Func<T> creator)
            where T : class
            => new LazyValueFunction<T>(creator);

        public static ILazyValue<T> CreateByServiceProvider<T>(IServiceProvider services)
            where T : class
            => new LazyValueServiceProvider<T>(services);
    }

    public class LazyValueFunction<T> : ILazyValue<T>
        where T : class {
        private Func<T>? _Creator;
        private enum State { Init, Created, Disposed }
        private int _StateValue;
        private T? _Value;

        public LazyValueFunction(Func<T> creator) {
            this._Creator = creator ?? throw new ArgumentNullException(nameof(creator));
            this.CurrentState = State.Init;
        }

        private State CurrentState {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (State)this._StateValue;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => this._StateValue = (int)value;
        }

        public T GetValue() {
            if (this.CurrentState == State.Created) {
                // happy path
                return this._Value!;
            } else {
                State state;
                lock (this) {
                    state = this.CurrentState;
                    var creator = this._Creator;
                    if ((state == State.Init) && (creator is object)) {
                        this._Value = creator();
                        this.CurrentState = state = State.Created;
                    }
                }
                if (state == State.Created) {
                    return this._Value!;
                } else if (state == State.Init) {
                    if (this._Creator is object) {
                        throw new InvalidOperationException($"LazyValueFunction<{typeof(T).FullName}> Not created. ");
                    } else {
                        throw new InvalidOperationException($"LazyValueFunction<{typeof(T).FullName}> Not created. Provider not given.");
                    }
                } else if (state == State.Disposed) {
                    throw new ObjectDisposedException($"LazyValueFunction<{typeof(T).FullName}>");
                } else {
                    throw new InvalidOperationException($"LazyValueFunction<{typeof(T).FullName}> Unknown State:{this.CurrentState}");
                }
            }
        }

        void System.IDisposable.Dispose() {
            var prevState = (State)System.Threading.Interlocked.Exchange(ref this._StateValue, (int)State.Disposed);
            if (prevState != State.Disposed) {
                this._Creator = default;
                this._Value = default;
            }
        }
    }

    public class LazyValueServiceProvider<T> : ILazyValue<T>
        where T : class {
        private enum State { Init, Created, Disposed }
        private IServiceProvider? _Provider;
        private int _StateValue;
        private T? _Value;

        public LazyValueServiceProvider(IServiceProvider provider) {
            this._Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            this.CurrentState = State.Init;
        }

        private State CurrentState {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (State)this._StateValue;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => this._StateValue = (int)value;
        }

        public T GetValue() {
            if (this.CurrentState == State.Created) {
                // happy path
                return this._Value!;
            } else {
                State state;
                lock (this) {
                    state = this.CurrentState;
                    var provider = this._Provider;
                    if ((state == State.Init) && (provider is object)) {
                        this._Value = provider.GetRequiredService<T>();
                        this.CurrentState = state = State.Created;
                    }
                }
                if (state == State.Created) {
                    return this._Value!;
                } else if (state == State.Init) {
                    if (this._Provider is object) {
                        throw new InvalidOperationException($"LazyValueServiceProvider<{typeof(T).FullName}> Not created. ");
                    } else {
                        throw new InvalidOperationException($"LazyValueServiceProvider<{typeof(T).FullName}> Not created. Provider not given.");
                    }
                } else if (state == State.Disposed) {
                    throw new ObjectDisposedException($"LazyValueServiceProvider<{typeof(T).FullName}>");
                } else {
                    throw new InvalidOperationException($"LazyValueServiceProvider<{typeof(T).FullName}> Unknown State:{this.CurrentState}");
                }
            }
        }
        void System.IDisposable.Dispose() {
            var prevState = (State)System.Threading.Interlocked.Exchange(ref this._StateValue, (int)State.Disposed);
            if (prevState != State.Disposed) {
                this._Provider = default;
                this._Value = default;
            }
        }
    }
}
