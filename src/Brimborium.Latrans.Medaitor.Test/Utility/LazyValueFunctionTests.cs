using System;

using Xunit;

namespace Brimborium.Latrans.Utility {
    //
    public class LazyValueFunctionTests {
        [Fact]
        public void LazyValueFunction_CreateByFunction_Test1() {
            var u = LazyValue.CreateByFunction(() => new Dummy() { Value = 1 });
            // first creation
            var d = u.GetValue();
            Assert.NotNull(d);
            Assert.Same(d, u.GetValue());
            Assert.Same(d, u.GetValue());

            // GetValue does not change
            Assert.Equal(1, u.GetValue().Value);
            d.Value = 2;
            Assert.Equal(2, u.GetValue().Value);

            u.Dispose();
            u.Dispose();
        }

        [Fact]
        public void LazyValueFunction_CreateByFunction_Test2_Dispose() {
            var u = LazyValue.CreateByFunction(() => new Dummy() { Value = 1 });
            Assert.NotNull(u.GetValue());
            u.Dispose();
            Assert.Throws<ObjectDisposedException>(() => { u.GetValue(); });

        }

        [Fact]
        public void LazyValueFunction_CreateByFunction_Test3_Dispose() {
            var u = LazyValue.CreateByFunction(() => new Dummy() { Value = 1 });

            // first creation
            Assert.NotNull(u.GetValue());

            u.Dispose();

            // dispose twice
            u.Dispose();
        }


        public class Dummy : IDisposable {
            public int Value;
            public void Dispose() {
                throw new NotSupportedException();
            }
        }
    }

}