using System;

using Xunit;

namespace Brimborium.Latrans.Utility {
    public class UsingValueFunctionTests {
        [Fact]
        public void UsingValueFunction_CreateByFunction_Test1() {
            var u = UsingValue.CreateByFunction(() => new Dummy() { Value = 1 });
            Assert.NotNull(u);

            // first creation
            var d = u.GetValue();
            Assert.NotNull(d);
            Assert.Same(d, u.GetValue());
            Assert.False(d.IsDisposed);

            // GetValue does not change
            Assert.Equal(1, u.GetValue().Value);
            d.Value = 2;
            Assert.False(u.GetValue().IsDisposed);
            Assert.Equal(2, u.GetValue().Value);

            u.Dispose();
            Assert.True(d.IsDisposed);
        }

        [Fact]
        public void UsingValueFunction_CreateByFunction_Test2_Dispose() {
            var u = UsingValue.CreateByFunction(() => new Dummy() { Value = 1 });
            Assert.NotNull(u);

            // first creation
            var d = u.GetValue();
            Assert.NotNull(d);
            Assert.Same(d, u.GetValue());
            Assert.False(d.IsDisposed);

            u.Dispose();
            Assert.Throws<ObjectDisposedException>(() => { u.GetValue(); });

        }

        [Fact]
        public void UsingValueFunction_CreateByFunction_Test3_Dispose() {
            var u = UsingValue.CreateByFunction(() => new Dummy() { Value = 1 });
            Assert.NotNull(u);

            // first creation
            var d = u.GetValue();
            Assert.NotNull(d);
            Assert.Same(d, u.GetValue());
            Assert.False(d.IsDisposed);

            u.Dispose();
            Assert.True(d.IsDisposed);

            // dispose twice
            u.Dispose();
            Assert.True(d.IsDisposed);
        }


        public class Dummy : IDisposable {
            public int Value;
            public bool IsDisposed;
            public void Dispose() {
                if (this.IsDisposed) {
                    // throw new InvalidOperationException();
                } else {
                    this.IsDisposed = true;
                }
            }
        }
    }
}