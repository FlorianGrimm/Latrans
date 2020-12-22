using Microsoft.Extensions.DependencyInjection;

using System;

using Xunit;
namespace Brimborium.Latrans.Utility {
    public class UsingValueServiceProviderTests {
        [Fact]
        public void UsingValueFunction_CreateByFunction_Test1() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddTransient<Dummy>();
            using (var serviceProvider = services.BuildServiceProvider()) {
                var u = UsingValue.CreateByServiceProvider<Dummy>(serviceProvider);
                Assert.NotNull(u);

                // first creation
                var d = u.GetValue();
                Assert.NotNull(d);
                Assert.Same(d, u.GetValue());
                Assert.False(d.IsDisposed);

                // GetValue does not change
                Assert.Equal(0, u.GetValue().Value);
                d.Value = 2;
                Assert.False(u.GetValue().IsDisposed);
                Assert.Equal(2, u.GetValue().Value);

                u.Dispose();
                Assert.True(d.IsDisposed);
            }
        }

        [Fact]
        public void UsingValueFunction_CreateByFunction_Test2_Dispose() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddTransient<Dummy>();
            using (var serviceProvider = services.BuildServiceProvider()) {
                var u = UsingValue.CreateByServiceProvider<Dummy>(serviceProvider);
                Assert.NotNull(u);

                // first creation
                var d = u.GetValue();
                Assert.NotNull(d);
                Assert.Same(d, u.GetValue());
                Assert.False(d.IsDisposed);

                u.Dispose();
                Assert.Throws<ObjectDisposedException>(() => { u.GetValue(); });
            }
        }

        [Fact]
        public void UsingValueFunction_CreateByFunction_Test3_Dispose() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddTransient<Dummy>();
            using (var serviceProvider = services.BuildServiceProvider()) {
                var u = UsingValue.CreateByServiceProvider<Dummy>(serviceProvider);
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