#pragma warning disable IDE0063 // Use simple 'using' statement
using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Utility;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace Brimborium.Latrans.Medaitor {
    public class MedaitorTests {
        [Fact]
        public void Medaitor_Test1() {
            var servicesWebApp = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            servicesWebApp.AddMedaitor();
            Assert.Contains(servicesWebApp, sd => typeof(IMedaitorAccess) == sd.ServiceType);
        }

        [Fact]
        public async Task Medaitor_Test2() {
            var servicesWebApp = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var servicesMediator = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            servicesWebApp.AddMedaitor();
            servicesMediator.AddTransient<IActivityHandler<TestRequest, TestResponse>, TestActivityHandler>();
            servicesMediator.AddHandler<TestActivityHandler>();

            using (var serviceProviderMediator = servicesMediator.BuildServiceProvider()) {
                using (var serviceProviderWebApp = servicesWebApp.BuildServiceProvider()) {
                    using (var scopeWebApp = serviceProviderWebApp.CreateScope()) {
                        var scopedProviderWebApp = scopeWebApp.ServiceProvider;
                        using (var localDisposables = new LocalDisposables()) {
                            var medaitorClient = localDisposables.AddUsingValue(scopedProviderWebApp.GetRequiredService<IMedaitorAccess>().GetMedaitorClient());

                            var request = new TestRequest() { A = 6, B = 7 };
                            var ctxt = medaitorClient.CreateContextByRequest(request);
                            await medaitorClient.SendAsync(ctxt, CancellationToken.None);
                            await medaitorClient.WaitForAsync(ctxt, CancellationToken.None);
                            //ctxt.GetResult();
                        }
                    }
                }
            }
        }

        public class TestRequest : IRequest<TestResponse> {
            public int A { get; set; }
            public int B { get; set; }
        }
        public class TestResponse : IResponseBase {
            public int R { get; set; }
        }

        public class TestActivityHandler : IActivityHandler<TestRequest, TestResponse> {
            public Task ExecuteAsync(
                IActivityContext<TestRequest, TestResponse> medaitorContext,
                CancellationToken cancellationToken) {
                var request = medaitorContext.Request;
                var response = new TestResponse() { R = request.A * request.B };
                medaitorContext.SetResponse(response);
                return Task.CompletedTask;
            }
        }
    }
}
