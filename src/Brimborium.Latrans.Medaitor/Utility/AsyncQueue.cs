using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Utility {
    public class AsyncQueue {
        public static AsyncQueue Create() {
            return new AsyncQueue(Task.CompletedTask);
        }

        private SpinLock _Lock;
        private Task _LastTask;

        public AsyncQueue(Task task) {
            _Lock = new SpinLock();
            _LastTask = task;
        }

        public void Next(
            Func<Task> asyncAction
            ) {
            bool lockTaken = false;
            this._Lock.Enter(ref lockTaken);
            try {
                var lastTask = (
                        this._LastTask.ContinueWith<Task>(
                            (previousTask, innerState) => {
                                if (innerState is null) {
                                    return Task.CompletedTask;
                                } else {
                                    var asyncAction = (Func<Task>) innerState;
                                    var nextTask = asyncAction();
                                    return nextTask;
                                }
                            },
                            asyncAction
                        ).Unwrap()
                    ) ?? (
                        Task.CompletedTask
                    );
                System.Threading.Volatile.Write(ref this._LastTask, lastTask);
            } finally {
                if (lockTaken) {
                    this._Lock.Exit(false);
                }
            }
        }


        public Task NextAsync(
            Func<Task> asyncAction
            ) {
            bool lockTaken = false;
            this._Lock.Enter(ref lockTaken);
            try {
                var lastTask = (
                        this._LastTask.ContinueWith<Task>(
                            (previousTask, innerState) => {
                                if (innerState is null) {
                                    return Task.CompletedTask;
                                } else {
                                    var asyncAction = (Func<Task>)innerState;
                                    var nextTask = asyncAction();
                                    return nextTask;
                                }
                            },
                            asyncAction
                        ).Unwrap()
                    ) ?? (
                        Task.CompletedTask
                    );
                System.Threading.Volatile.Write(ref this._LastTask, lastTask);
                return lastTask;
            } finally {
                if (lockTaken) {
                    this._Lock.Exit(false);
                }
            }
        }

        public void Next<TState>(
            Func<TState, Task> asyncAction,
            TState state
            ) {
            bool lockTaken = false;
            this._Lock.Enter(ref lockTaken);
            try {
                var lastTask = (
                        this._LastTask.ContinueWith<Task>(
                            (previousTask, innerState) => {
                                if (innerState is null) {
                                    return Task.CompletedTask;
                                } else { 
                                    var innerStateT = ( (Func<TState, Task> asyncAction, TState state) ) innerState;
                                    var nextTask = innerStateT.asyncAction(innerStateT.state);
                                    return nextTask;
                                }
                            },
                            (asyncAction, state)
                        ).Unwrap()
                    ) ?? (
                        Task.CompletedTask
                    );
                System.Threading.Volatile.Write(ref this._LastTask, lastTask);
            } finally {
                if (lockTaken) {
                    this._Lock.Exit(false);
                }
            }
        }

        public Task NextAsync<TState>(
            Func<TState, Task> asyncAction,
            TState state
            ) {
            bool lockTaken = false;
            this._Lock.Enter(ref lockTaken);
            try {
                var lastTask = (
                        this._LastTask.ContinueWith<Task>(
                            (previousTask, innerState) => {
                                if (innerState is null) {
                                    return Task.CompletedTask;
                                } else {
                                    var innerStateT = ((Func<TState, Task> asyncAction, TState state))innerState;
                                    var nextTask = innerStateT.asyncAction(innerStateT.state);
                                    return nextTask;
                                }
                            },
                            (asyncAction, state)
                        ).Unwrap()
                    ) ?? (
                        Task.CompletedTask
                    );
                System.Threading.Volatile.Write(ref this._LastTask, lastTask);
                return lastTask;
            } finally {
                if (lockTaken) {
                    this._Lock.Exit(false);
                }
            }
        }
    }
}
