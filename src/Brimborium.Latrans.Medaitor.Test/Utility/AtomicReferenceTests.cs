using System;
using System.Diagnostics;
using System.Threading;

using Xunit;

namespace Brimborium.Latrans.Utility {
    public class AtomicReferenceTests {
        [Fact]
        public void AtomicReference_1Mutate() {
            var sut = new AtomicReference<D>(new D(1));
            Assert.Equal(1, sut.Value.Data1);
            sut.Mutate((v) => new D(v.Data1 + 1));
            Assert.Equal(2, sut.Value.Data1);
        }

        [Fact]
        public void AtomicReference_2Mutate1() {
            var sut = new AtomicReference<D>(new D(1));
            Assert.Equal(1, sut.Value.Data1);
            sut.Mutate1(1, (a, v) => new D(v.Data1 + a));
            Assert.Equal(2, sut.Value.Data1);
            sut.Mutate1(2, (a, v) => new D(v.Data1 + a));
            Assert.Equal(4, sut.Value.Data1);
        }

        [Fact]
        public void AtomicReference_3Mutate1Threaded() {
            using var startEvent = new ManualResetEvent(false);
            using var stopEvent1 = new ManualResetEvent(false);
            using var stopEvent2 = new ManualResetEvent(false);
            var sut = new AtomicReference<D>(new D(0));
            int retries = 0;
            ThreadPool.QueueUserWorkItem((a) => {
                try {
                    startEvent.WaitOne();
                    for (int i = 1; i < 1000; i++) {
                        sut.Mutate1(
                            a,
                            (a, v) => new D(v.Data1 + a),
                            (a) => System.Threading.Interlocked.Increment(ref retries)
                            );
                    }
                } finally {
                    stopEvent1.Set();
                }
            }, 1, true);
            //
            ThreadPool.QueueUserWorkItem((a) => {
                try {
                    startEvent.WaitOne();
                    for (int i = 1; i < 1000; i++) {
                        sut.Mutate1(
                            a,
                            (a, v) => new D(v.Data1 + a),
                            (a) => System.Threading.Interlocked.Increment(ref retries)
                            );
                    }
                } finally {
                    stopEvent2.Set();
                }
            }, 1_000, true);
            //
            startEvent.Set();
            stopEvent1.WaitOne(TimeSpan.FromMinutes(1));
            stopEvent2.WaitOne(TimeSpan.FromMinutes(1));
            Assert.Equal(999_999, sut.Value.Data1);
            var infos = sut.GetInfos();
            Assert.True(infos.LoopsCount == retries, $"infos.LoopsCount: {infos.LoopsCount}");
            Assert.True(retries < 10, $"retries: {retries}"); // weak test condition
        }
        [Fact]
        public void AtomicReference_4Mutate1Threaded() {
            using var startEvent = new ManualResetEvent(false);
            using var stopEvent1 = new ManualResetEvent(false);
            using var stopEvent2 = new ManualResetEvent(false);
            using var stopEvent3 = new ManualResetEvent(false);
            var sut = new AtomicReference<D>(new D(0));
            int retries = 0;
            //
            ThreadPool.QueueUserWorkItem((a) => {
                try {
                    startEvent.WaitOne();
                    for (int i = 1; i < 1000; i++) {
                        sut.Mutate1(
                            a,
                            (a, v) => new D(v.Data1 + a),
                            (a) => System.Threading.Interlocked.Increment(ref retries)
                            );
                    }
                } finally {
                    stopEvent1.Set();
                }
            }, 1, true);
            //
            ThreadPool.QueueUserWorkItem((a) => {
                try {
                    startEvent.WaitOne();
                    for (int i = 1; i < 1000; i++) {
                        sut.Mutate1(
                            a,
                            (a, v) => new D(v.Data1 + a),
                            (a) => System.Threading.Interlocked.Increment(ref retries)
                            );
                    }
                } finally {
                    stopEvent2.Set();
                }
            }, 1_000, true);
            //
            ThreadPool.QueueUserWorkItem((a) => {
                try {
                    startEvent.WaitOne();
                    for (int i = 1; i < 1000; i++) {
                        sut.Mutate1(
                            a,
                            (a, v) => new D(v.Data1 + a),
                            (a) => System.Threading.Interlocked.Increment(ref retries)
                            );
                    }
                } finally {
                    stopEvent3.Set();
                }
            }, 1_000_000, true);
            startEvent.Set();
            stopEvent1.WaitOne(TimeSpan.FromMinutes(1));
            stopEvent2.WaitOne(TimeSpan.FromMinutes(1));
            stopEvent3.WaitOne(TimeSpan.FromMinutes(1));
            Assert.Equal(999_999_999, sut.Value.Data1);
            Assert.True(retries < 1_000, $"retries: {retries}"); // weak test condition
        }
        class D {
            [DebuggerStepThrough]
            public D(int data1) {
                this.Data1 = data1;
            }
            public int Data1 { get; }
            public override string ToString()
                => this.Data1.ToString();
        }

    }
}
