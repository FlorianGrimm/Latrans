using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Brimborium.Latrans.Utility {
    /// <summary>Holds a reference to an immutable class and updates it atomically.</summary>
    /// <typeparam name="T">An immutable class to reference.</typeparam>
    public class AtomicReference<T> where T : class {
        private int _LockEnabled;
        private int _LoopsCount;
        private T _Value;

        public AtomicReference(T initialValue) {
            this._Value = initialValue;
        }

        /// <summary>Gets the current value of this instance.</summary>
        public T Value {
            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            get => this._Value;
        }

        public T SetValue(T value) 
            => System.Threading.Interlocked.Exchange(ref this._Value, value);

        /// <summary>Atomically updates the value of this instance.</summary>
        /// <param name="mutator">A pure function to compute a new value based on the current value of the instance.
        /// This function may be called more than once.</param>
        /// <param name="dispose">A function to dispose a value that is not used.</param>
        /// <returns>The previous value that was used to generate the resulting new value.</returns>
        public T Mutate(Func<T, T> mutator, Action<T>? dispose = default) {
            T oldValue;
            int loops = 0;
            bool lockEnabled = false;
            try {
                while (true) {
                    if (!lockEnabled) {
                        // prevent endless loop
                        if (loops > 10) {
                            System.Threading.Interlocked.Increment(ref this._LockEnabled);
                            lockEnabled = true;
                        } else if (this._LockEnabled > 0) {
                            lockEnabled = true;
                        }
                    }
                    T nextValue;
                    T currentVal;
                    oldValue = System.Threading.Volatile.Read(ref this._Value);
                    if (lockEnabled) {
                        lock (this) {
                            nextValue = mutator(oldValue);
                            currentVal = System.Threading.Interlocked.CompareExchange(ref _Value, nextValue, oldValue);
                            if (ReferenceEquals(currentVal, oldValue)) {
                                return oldValue;
                            }
                        }
                    } else {
                        nextValue = mutator(oldValue);
                        currentVal = System.Threading.Interlocked.CompareExchange(ref _Value, nextValue, oldValue);
                        if (ReferenceEquals(currentVal, oldValue)) {
                            return oldValue;
                        }
                    }

                    {
                        oldValue = currentVal;
                        if (dispose is object) {
                            dispose(nextValue);
                        }
                        loops++;
                        System.Threading.Interlocked.Increment(ref this._LoopsCount);
                    }
                }
            } finally {
                if (lockEnabled) {
                    System.Threading.Interlocked.Decrement(ref this._LockEnabled);
                }
            }
        }

        public T Mutate1<A>(A argument, Func<A, T, T> mutator, Action<T>? dispose = default) {
            T oldValue;
            int loops = 0;
            bool lockEnabled = false;
            try {
                while (true) {
                    if (!lockEnabled) {
                        if (loops > 10) {
                            System.Threading.Interlocked.Increment(ref this._LockEnabled);
                            lockEnabled = true;
                        } else if (this._LockEnabled > 0) {
                            lockEnabled = true;
                        }
                    }
                    T nextValue;
                    T currentVal;
                    if (lockEnabled) {
                        lock (this) {
                            oldValue = this._Value;
                            nextValue = mutator(argument, oldValue);
                            System.Threading.Interlocked.MemoryBarrier();
                            currentVal = System.Threading.Interlocked.CompareExchange(ref _Value, nextValue, oldValue);
                            if (ReferenceEquals(currentVal, oldValue)) {
                                return oldValue;
                            }
                        }
                    } else {
                        oldValue = this._Value;
                        nextValue = mutator(argument, oldValue);
                        System.Threading.Interlocked.MemoryBarrier();
                        currentVal = System.Threading.Interlocked.CompareExchange(ref _Value, nextValue, oldValue);
                        if (ReferenceEquals(currentVal, oldValue)) {
                            return oldValue;
                        }
                    }

                    {
                        oldValue = currentVal;
                        if (dispose is object) {
                            dispose(nextValue);
                        }
                        loops++;
                        System.Threading.Interlocked.Increment(ref this._LoopsCount);
                    }
                }
            } finally {
                if (lockEnabled) {
                    System.Threading.Interlocked.Decrement(ref this._LockEnabled);
                }
            }
        }

        /// <summary>Debug info</summary>
        /// <returns>Current info</returns>
        public AtomicReferenceInfo GetInfos()
            => new AtomicReferenceInfo() { 
                LockEnabled = this._LockEnabled,
                LoopsCount = this._LoopsCount
            };

        public struct AtomicReferenceInfo {
            public int LockEnabled;
            public int LoopsCount;
        }
    }
}
