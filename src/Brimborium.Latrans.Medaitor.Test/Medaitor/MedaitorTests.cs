#pragma warning disable IDE0063 // Use simple 'using' statement
using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Utility;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace Brimborium.Latrans.Mediator {
    public class MedaitorTests {
        [Fact]
        public void Medaitor_Test1() {
            var servicesWebApp = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var b = servicesWebApp.AddLatransMedaitor();
            Assert.Contains(servicesWebApp, sd => typeof(IMediatorClientFactory) == sd.ServiceType);
            Assert.Contains(servicesWebApp, sd => typeof(IMediatorClient) == sd.ServiceType);
            Assert.Contains(servicesWebApp, sd => typeof(IMediatorService) == sd.ServiceType);
        }

        [Fact]
        public void Medaitor_Test2() {
            var servicesWebApp = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var servicesMediator = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var b = servicesWebApp.AddLatransMedaitor();
            b.AddHandler<TestActivityHandler>();
            
            Assert.Contains(b.Services, sd => typeof(IActivityHandler<TestRequest, TestResponse>) == sd.ServiceType);
        }

        [Fact]
        public void Medaitor_Test3() {
            var servicesWebApp = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var servicesMediator = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var b = servicesWebApp.AddLatransMedaitor();
            b.AddHandler<TestActivityHandler>();

            using (var serviceProviderMediator = servicesMediator.BuildServiceProvider()) {
                using (var serviceProviderWebApp = servicesWebApp.BuildServiceProvider()) {
                    using (var scopeWebApp = serviceProviderWebApp.CreateScope()) {
                        var scopedProviderWebApp = scopeWebApp.ServiceProvider;
                        var medaitorClient = scopedProviderWebApp.GetRequiredService<IMediatorClientFactory>().GetMedaitorClient();

                        var request = new TestRequest() { A = 6, B = 7 };
                        var ctxt = medaitorClient.CreateContextByRequest(request);
                        Assert.Same(request, ctxt.Request);
                    }
                }
            }
        }

        [Fact]
        public async Task Medaitor_Test4() {
            var servicesWebApp = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var servicesMediator = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var b = servicesWebApp.AddLatransMedaitor();
            b.AddHandler<TestActivityHandler>();

            using (var serviceProviderMediator = servicesMediator.BuildServiceProvider()) {
                using (var serviceProviderWebApp = servicesWebApp.BuildServiceProvider()) {
                    using (var scopeWebApp = serviceProviderWebApp.CreateScope()) {
                        var scopedProviderWebApp = scopeWebApp.ServiceProvider;
                        var medaitorClient = scopedProviderWebApp.GetRequiredService<IMediatorClientFactory>().GetMedaitorClient();

                        var request = new TestRequest() { A = 6, B = 7 };
                        var ctxt = medaitorClient.CreateContextByRequest(request);
                        await medaitorClient.SendAsync(ctxt, CancellationToken.None);
                        await medaitorClient.WaitForAsync(ctxt, null, CancellationToken.None);
                        var activityResponse = await ctxt.GetActivityResponseAsync();
                        Assert.NotNull(activityResponse as OkResultActivityResponse<TestResponse>);
                    }
                }
            }
        }

        [Fact]
        public async Task Medaitor_Test5() {
            var servicesWebApp = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var servicesMediator = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var b = servicesWebApp.AddLatransMedaitor();
            b.AddHandler<TestActivityHandler>();

            using (var serviceProviderMediator = servicesMediator.BuildServiceProvider()) {
                using (var serviceProviderWebApp = servicesWebApp.BuildServiceProvider()) {
                    using (var scopeWebApp = serviceProviderWebApp.CreateScope()) {
                        var scopedProviderWebApp = scopeWebApp.ServiceProvider;
                        var medaitorClient = scopedProviderWebApp.GetRequiredService<IMediatorClientFactory>().GetMedaitorClient();

                        var request = new TestRequest() { A = 6, B = 7 };
                        var connectedClient = await medaitorClient.ConnectAsync(request, CancellationToken.None);
                        //var ctxt = medaitorClient.CreateContextByRequest(request);
                        //await medaitorClient.SendAsync(ctxt, CancellationToken.None);
                        //await medaitorClient.WaitForAsync(ctxt, null, CancellationToken.None);
                        //var activityResponse = await ctxt.GetActivityResponseAsync();
                        //Assert.NotNull(activityResponse as OkResultActivityResponse<TestResponse>);
                    }
                }
            }
        }

        [Fact]
        public async Task Medaitor_Test6() {
            var servicesWebApp = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var servicesMediator = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var b = servicesWebApp.AddLatransMedaitor();
            b.AddHandler<TestActivityHandler>();

            using (var serviceProviderMediator = servicesMediator.BuildServiceProvider()) {
                using (var serviceProviderWebApp = servicesWebApp.BuildServiceProvider()) {
                    using (var scopeWebApp = serviceProviderWebApp.CreateScope()) {
                        var scopedProviderWebApp = scopeWebApp.ServiceProvider;
                        var medaitorClient = scopedProviderWebApp.GetRequiredService<IMediatorClientFactory>().GetMedaitorClient();

                        var request = new TestRequest() { A = 6, B = 7 };
                        var connectedClient = await medaitorClient.ConnectAsync(request, CancellationToken.None);
                        var activityResponse =await connectedClient.WaitForAsync(null, CancellationToken.None);
                        //var ctxt = medaitorClient.CreateContextByRequest(request);
                        //await medaitorClient.SendAsync(ctxt, CancellationToken.None);
                        //await medaitorClient.WaitForAsync(ctxt, null, CancellationToken.None);
                        //var activityResponse = await ctxt.GetActivityResponseAsync();
                        //Assert.NotNull(activityResponse as OkResultActivityResponse<TestResponse>);
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

        public class TestActivityHandler : ActivityHandlerBase<TestRequest, TestResponse> {
            public override Task ExecuteAsync(
                IActivityContext<TestRequest, TestResponse> activityContext,
                CancellationToken cancellationToken) {
                var request = activityContext.Request;
                var response = new TestResponse() { R = request.A * request.B };
                activityContext.SetResponse(response);
                return Task.CompletedTask;
            }
        }
    }
}
