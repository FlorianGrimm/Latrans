using System;
using System.Collections.Generic;
using System.Text;

namespace Brimborium.Latrans.Utility {
    /// <summary>Holds a reference to an immutable class and updates it atomically.</summary>
    /// <typeparam name="T">An immutable class to reference.</typeparam>
    class AtomicReference<T> where T : class {
        private volatile T _Value;

        public AtomicReference(T initialValue) {
            this._Value = initialValue;
        }

        /// <summary>Gets the current value of this instance.</summary>
        public T Value => this._Value;

        /// <summary>Atomically updates the value of this instance.</summary>
        /// <param name="mutator">A pure function to compute a new value based on the current value of the instance.
        /// This function may be called more than once.</param>
        /// <param name="dispose">A function to dispose a value that is not used.</param>
        /// <returns>The previous value that was used to generate the resulting new value.</returns>
        public T Mutate(Func<T, T> mutator, Action<T> dispose=default) {
            T oldValue = this._Value;
            while (true) {
                T nextValue = mutator(oldValue);
#pragma warning disable 420
                T currentVal = System.Threading.Interlocked.CompareExchange(ref _Value, nextValue, oldValue);
#pragma warning restore 420

                if (ReferenceEquals(currentVal, oldValue)) {
                    return oldValue;
                } else {
                    oldValue = currentVal;
                    if (dispose is object) {
                        dispose(nextValue);
                    }
                }
            }
        }
        public T Mutate1<A>(A argument, Func<A, T, T> mutator, Action<T> dispose = default) {
            T oldValue = this._Value;
            while (true) {
                T nextValue = mutator(argument, oldValue);
#pragma warning disable 420
                T currentVal = System.Threading.Interlocked.CompareExchange(ref _Value, nextValue, oldValue);
#pragma warning restore 420

                if (ReferenceEquals(currentVal, oldValue)) {
                    return oldValue;
                } else {
                    oldValue = currentVal;
                    if (dispose is object) {
                        dispose(nextValue);
                    }
                }
            }
        }
    }
}
