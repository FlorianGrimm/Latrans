using System;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace Brimborium.Latrans.Utility {
    public class LazyValueServiceProviderTests {
        [Fact]
        public void LazyValueFunction_CreateByFunction_Test1() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddTransient<Dummy>();
            using (var serviceProvider = services.BuildServiceProvider()) {
                var u = LazyValue.CreateByServiceProvider<Dummy>(serviceProvider);
                Assert.NotNull(u);

                // first creation
                var d = u.GetValue();
                Assert.NotNull(d);
                Assert.Same(d, u.GetValue());

                // d is no more used
                Assert.NotNull(u.GetValue());

                // GetValue does not change
                Assert.Equal(0, u.GetValue().Value);

                d.Value = 2;
                Assert.Equal(2, u.GetValue().Value);

                u.Dispose();
                u.Dispose();

                d.DisableDispose();
            }
        }

        [Fact]
        public void LazyValueFunction_CreateByFunction_Test2_Dispose() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddTransient<Dummy>();
            using (var serviceProvider = services.BuildServiceProvider()) {
                var u = LazyValue.CreateByServiceProvider<Dummy>(serviceProvider);
                Assert.NotNull(u);

                // first creation
                Dummy d = u.GetValue();
                Assert.NotNull(d);
                Assert.Same(d, u.GetValue());

                u.Dispose();

                // dispose twice
                u.Dispose();

                d.DisableDispose();
            }
        }


        public class Dummy : IDisposable {
            public int Value;
            private bool _DisableDispose;

            public void Dispose() {
                if (this._DisableDispose) {
                    //
                } else {
                    throw new NotSupportedException();
                }
            }

            internal void DisableDispose() {
                this._DisableDispose = true;
            }
        }
    }

}