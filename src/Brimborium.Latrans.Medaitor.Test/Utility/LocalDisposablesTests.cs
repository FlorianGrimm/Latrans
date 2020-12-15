using System;

using Xunit;

namespace Brimborium.Latrans.Utility {
    public class LocalDisposablesTests {
        public void LocalDisposables_Test1() {
            var d = new TestingIDisposable();
            Assert.False(d.IsDisposed);
            var sut = new LocalDisposables();
            var d2 = sut.Add(d);
            Assert.Same(d, d2);
            sut.Dispose();
            Assert.True(d.IsDisposed);
        }

        public void LocalDisposables_Test2() {
            var sut = new LocalDisposables();
            var d = sut.AddUsingValue(UsingValue.CreateByFunction(() => new TestingIDisposable()));
            Assert.False(d.IsDisposed);
            sut.Dispose();
            Assert.True(d.IsDisposed);
        }

        public class TestingIDisposable : IDisposable {
            public int Value;
            public bool IsDisposed;
            public void Dispose() {
                this.IsDisposed = true;
            }
        }
    }
}