#pragma warning disable CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;

using BenchmarkDotNet.Attributes;

using Brimborium.Latrans.Collections;
using Brimborium.Latrans.Utility;

namespace Benchmark {
    public class MultiThreaded {
        const int cntAdd = 1_000;
        const int cntRead = 100;

        private static D[]? datas1;
        private static D[]? datas2;

        private static (D[] datas1, D[] datas2) GetDatas() {
            if (datas1 is null) {
                datas1 = System.Linq.Enumerable.Range(1_000_001, cntAdd).Select(i => new D(i)).ToArray();
            }
            if (datas2 is null) {
                datas2 = System.Linq.Enumerable.Range(2_000_001, cntAdd).Select(i => new D(i)).ToArray();
            }
            return (datas1!, datas2!);
        }

        private const int N = 1000;
        private readonly byte[] data;
        private readonly SHA256 sha256 = SHA256.Create();
        public MultiThreaded() {
            data = new byte[N];
            new Random(42).NextBytes(data);
        }
        private int eatTime() {
            //sha256.ComputeHash(data);
            unchecked {
                int sum = 0;
                foreach(var b in data) {
                    sum += b;
                }
                return sum;
            }
        }

        private static void Run(Action action1, Action action2) {
            using var evtStart = new ManualResetEvent(false);
            using var evtT1 = new ManualResetEvent(false);
            using var evtT2 = new ManualResetEvent(false);
            System.Exception? error = null;
            var t1 = new Thread((ThreadStart)delegate () {
                evtStart.WaitOne();
                try {
                    action1();
                } catch (System.Exception e) {
                    error = e;
                    evtT1.Set();
                } finally {
                    evtT1.Set();
                }
            });
            var t2 = new Thread((ThreadStart)delegate () {
                evtStart.WaitOne();
                try {
                    action2();
                } catch (System.Exception e) {
                    error = e;
                    evtT2.Set();
                } finally {
                    evtT2.Set();
                }
            });
            t1.Start();
            t2.Start();
            evtStart.Set();

            evtT1.WaitOne(TimeSpan.FromMinutes(5));
            evtT2.WaitOne(TimeSpan.FromMinutes(5));

            if (error is object) {
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw();
            }
        }

        [Benchmark(Baseline = true)]
        public void Scenario0EatTime() {
            var datas = GetDatas();

            Run(
                () => {
                    foreach (var d in datas.datas1) {
                        lock (this) {
                            System.Threading.Interlocked.MemoryBarrier();
                        }
                        eatTime();
                    }
                },
                () => {
                    foreach (var d in datas.datas2) {
                        lock (this) {
                            System.Threading.Interlocked.MemoryBarrier();
                        }
                        eatTime();
                    }
                }
                );

            for (int idx = 1; idx <= cntRead; idx++) {
                if (datas.datas1.Length != cntAdd) { throw new Exception($"{datas.datas1.Length} != {cntAdd}"); }
                if (datas.datas2.Length != cntAdd) { throw new Exception($"{datas.datas2.Length} != {cntAdd}"); }
            }
        }

        [Benchmark()]
        public void Scenario1List() {
            var lst = new List<D>();
            var datas = GetDatas();

            Run(
                () => {
                    foreach (var d in datas.datas1) {
                        lock (this) {
                            lst.Add(d);
                            System.Threading.Interlocked.MemoryBarrier();
                        }
                        eatTime();
                    }
                },
                () => {
                    foreach (var d in datas.datas2) {
                        lock (this) {
                            lst.Add(d);
                            System.Threading.Interlocked.MemoryBarrier();
                        }
                        eatTime();
                    }
                }
                );

            for (int idx = 1; idx <= cntRead; idx++) {
                var a = lst.ToArray();
                if (a.Length != 2 * cntAdd) { throw new Exception($"{a.Length} != 2*{cntAdd}"); }
            }
        }

        [Benchmark()]
        public void Scenario2ListCopied() {
            var lst = new List<D>();
            var datas = GetDatas();

            Run(
                () => {
                    foreach (var d in datas.datas1) {
                        lock (this) {
                            var ll = System.Threading.Volatile.Read(ref lst);
                            System.Threading.Interlocked.MemoryBarrier();
                            ll = ll.ToList();
                            ll.Add(d);
                            System.Threading.Volatile.Write(ref lst, ll);
                        }
                        eatTime();
                    }
                },
                () => {
                    foreach (var d in datas.datas2) {
                        lock (this) {
                            var ll = System.Threading.Volatile.Read(ref lst);
                            System.Threading.Interlocked.MemoryBarrier();
                            ll = ll.ToList();
                            ll.Add(d);
                            System.Threading.Volatile.Write(ref lst, ll);
                        }
                        eatTime();
                    }
                }
                );
            System.Threading.Interlocked.MemoryBarrier();
            for (int idx = 1; idx <= cntRead; idx++) {
                var a = lst.ToArray();
                if (a.Length != 2 * cntAdd) { throw new Exception($"{a.Length} != 2*{cntAdd}"); }
            }
        }

        [Benchmark]
        public void Scenario3ImmutableList() {
            var lst = System.Collections.Immutable.ImmutableList<D>.Empty;
            var datas = GetDatas();

            Run(
               () => {
                   foreach (var d in datas.datas1) {
                       lock (this) {
                           var ll = System.Threading.Volatile.Read(ref lst);
                           ll = ll.Add(d);
                           System.Threading.Volatile.Write(ref lst, ll);
                           System.Threading.Interlocked.MemoryBarrier();
                       }
                       eatTime();
                   }
               },
               () => {
                   foreach (var d in datas.datas2) {
                       lock (this) {
                           var ll = System.Threading.Volatile.Read(ref lst);
                           ll = ll.Add(d);
                           System.Threading.Volatile.Write(ref lst, ll);
                           System.Threading.Interlocked.MemoryBarrier();
                       }
                       eatTime();
                   }
               }
               );

            for (int idx = 1; idx <= cntRead; idx++) {
                var a = lst.ToArray();
                if (a.Length != 2 * cntAdd) { throw new Exception($"{a.Length} != 2*{cntAdd}"); }
            }
        }

        [Benchmark]
        public void Scenario4ImListLock() {
            var lst = new ImList<D>();
            var datas = GetDatas();

            Run(
               () => {
                   foreach (var d in datas.datas1) {
                       lock (this) {
                           var ll = System.Threading.Volatile.Read(ref lst);
                           ll = ll.Add(d);
                           System.Threading.Volatile.Write(ref lst, ll);
                           System.Threading.Interlocked.MemoryBarrier();
                       }
                       eatTime();
                   }
               },
               () => {
                   foreach (var d in datas.datas2) {
                       lock (this) {
                           var ll = System.Threading.Volatile.Read(ref lst);
                           ll = ll.Add(d);
                           System.Threading.Volatile.Write(ref lst, ll);
                           System.Threading.Interlocked.MemoryBarrier();
                       }
                       eatTime();
                   }
               }
               );

            for (int idx = 1; idx <= cntRead; idx++) {
                var a = lst.ToArray();
                if (a.Length != 2 * cntAdd) { throw new Exception($"{a.Length} != 2*{cntAdd}"); }
            }
        }

        [Benchmark]
        public void Scenario5ImListAtomic() {
            var lst = new AtomicReference<ImList<D>>(new ImList<D>());
            var datas = GetDatas();

            Run(
               () => {
                   foreach (var d in datas.datas1) {
                       lst.Mutate1(d, (item, lst) => lst.Add(item));
                       eatTime();
                   }
               },
               () => {
                   foreach (var d in datas.datas2) {
                       lst.Mutate1(d, (item, lst) => lst.Add(item));
                       eatTime();
                   }
               }
               );

            for (int idx = 1; idx <= cntRead; idx++) {
                var a = lst.Value.ToArray();
                if (a.Length != 2*cntAdd) { throw new Exception($"{a.Length} != 2*{cntAdd}"); }
            }
        }


        class D {
            public D(int data1) {
                this.Data1 = data1;
            }
            public int Data1 { get; }
            public override string ToString()
                => this.Data1.ToString();
        }
    }
}