using System;

using Xunit;

namespace Brimborium.Latrans.Utility {
    public class UsingValueTests {
        [Fact]
        public void UsingValue_CreateByFunction_Test1() {
            TestingIDisposable d = null;
            var u = UsingValue.CreateByFunction(() => d = new TestingIDisposable() { Value = 1 });

            // first creation
            Assert.Null(d);
            Assert.NotNull(u.GetValue());
            Assert.Same(d, u.GetValue());
            Assert.False(d.IsDisposed);
            d = null;

            // d is no more used
            Assert.NotNull(u.GetValue());

            // GetValue does not change
            Assert.Equal(1, u.GetValue().Value);
            u.GetValue().Value = 2;
            Assert.False(u.GetValue().IsDisposed);
            Assert.Equal(2, u.GetValue().Value);

            u.Dispose();
        }

        [Fact]
        public void UsingValue_CreateByFunction_Test2_Dispose() {
            TestingIDisposable d = null;
            var u = UsingValue.CreateByFunction(() => d = new TestingIDisposable() { Value = 1 });
            Assert.NotNull(u.GetValue());
            u.Dispose();
            Assert.Throws<ObjectDisposedException>(() => { u.GetValue(); });

        }

        [Fact]
        public void UsingValue_CreateByFunction_Test3_Dispose() {
            TestingIDisposable d = null;
            var u = UsingValue.CreateByFunction(() => d = new TestingIDisposable() { Value = 1 });

            // first creation
            Assert.Null(d);
            Assert.NotNull(u.GetValue());
            Assert.Same(d, u.GetValue());
            Assert.False(d.IsDisposed);

            u.Dispose();
            Assert.True(d.IsDisposed);

            // dispose twice
            u.Dispose();
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