
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Runtime.CompilerServices;

namespace Brimborium.Latrans.Utility {
    public static class UsingValue {
        public static IUsingValue<T> CreateByFunction<T>(Func<T> creator)
            where T : class, IDisposable
            => new UsingValueFunction<T>(creator);

        public static IUsingValue<T> CreateByServiceProvider<T>(IServiceProvider services)
            where T : class, IDisposable
            => new UsingValueServiceProvider<T>(services);
    }

    public class UsingValueFunction<T> : IUsingValue<T>
        where T : class, IDisposable {
        private Func<T> _Creator;
        private enum State { Init, Created, Disposed }
        private int _StateValue;
        private T _Value;
        
        public UsingValueFunction(Func<T> creator) {
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
            if (this.CurrentState==State.Created) {
                // happy path
                return this._Value;
            } else {
                State state;
                lock (this) {
                    state = this.CurrentState;
                    if (state == State.Init) {
                        this._Value = this._Creator();
                        this.CurrentState = State.Created;
                    }
                }
                if ((state == State.Init)
                    || (state == State.Created)) {
                    return this._Value;
                } else if (state == State.Disposed) {
                    throw new ObjectDisposedException($"UsingValueFunction<{typeof(T).FullName}>");
                } else {
                    throw new InvalidOperationException($"UsingValueFunction<{typeof(T).FullName}> Unknown State:{this.CurrentState}");
                }
            }
        }
        public void Dispose() {
            this.Disposing();
            GC.SuppressFinalize(this);
        }
        ~UsingValueFunction() {
            this.Disposing();
        }
        private void Disposing() {
            var prevState = (State)System.Threading.Interlocked.Exchange(ref this._StateValue, (int)State.Disposed);
            if (prevState == State.Created) {
                this._Creator = default;
                var oldValue = System.Threading.Interlocked.Exchange<T>(ref this._Value, default);
                oldValue?.Dispose();
            }
        }
    }
    public class UsingValueServiceProvider<T> : IUsingValue<T>
        where T : class, IDisposable {
        private IServiceProvider _Provider;
        private enum State { Init, Created, Disposed }
        private int _StateValue;
        private T _Value;

        public UsingValueServiceProvider(IServiceProvider provider) {
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
                return this._Value;
            } else {
                State state;
                lock (this) {
                    state = this.CurrentState;
                    if (state == State.Init) {
                        this._Value = this._Provider.GetRequiredService<T>();
                        this.CurrentState = State.Created;
                    }
                }
                if ((state == State.Init)
                    || (state == State.Created)) {
                    return this._Value;
                } else if (state == State.Disposed) {
                    throw new ObjectDisposedException($"UsingValueFunction<{typeof(T).FullName}>");
                } else {
                    throw new InvalidOperationException($"UsingValueFunction<{typeof(T).FullName}> Unknown State:{this.CurrentState}");
                }
            }
        }
        public void Dispose() {
            this.Disposing();
            GC.SuppressFinalize(this);
        }
        ~UsingValueServiceProvider() {
            this.Disposing();
        }
        private void Disposing() {
            var prevState = (State)System.Threading.Interlocked.Exchange(ref this._StateValue, (int)State.Disposed);
            if (prevState == State.Created) {
                this._Provider = default;
                var oldValue = System.Threading.Interlocked.Exchange<T>(ref this._Value, default);
                oldValue?.Dispose();
            }
        }
    }
}
