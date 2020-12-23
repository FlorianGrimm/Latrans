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
        public void Medaitor_1AddLatransMedaitor() {
            var servicesWebApp = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var b = servicesWebApp.AddLatransMedaitor();
            //b.Build();
            Assert.Contains(servicesWebApp, sd => typeof(IMediatorClientFactory) == sd.ServiceType);
            Assert.Contains(servicesWebApp, sd => typeof(IMediatorClient) == sd.ServiceType);
            Assert.Contains(servicesWebApp, sd => typeof(IMediatorService) == sd.ServiceType);
        }

        [Fact]
        public void Medaitor_2AddHandler() {
            var servicesWebApp = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var servicesMediator = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var b = servicesWebApp.AddLatransMedaitor();
            b.AddActivityHandler<TestActivityHandler1>()
                .Build();

            Assert.Contains(b.Services, sd => typeof(IActivityHandler<TestRequest1, TestResponse1>) == sd.ServiceType);
        }

        [Fact]
        public async Task Medaitor_3ConnectAsync() {
            var servicesWebApp = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var servicesMediator = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            servicesWebApp.AddLatransMedaitor()
                .AddActivityHandler<TestActivityHandler1>()
                .AddActivityHandler<TestActivityHandler2>()
                .Build();

            IMediatorClientConnected<TestRequest1> connectedClient = null;
            MediatorClientConnected<TestRequest1, TestResponse1> testConnectedClient = null;
            using (var serviceProviderMediator = servicesMediator.BuildServiceProvider()) {
                using (var serviceProviderWebApp = servicesWebApp.BuildServiceProvider()) {
                    using (var scopeWebApp = serviceProviderWebApp.CreateScope()) {
                        var scopedProviderWebApp = scopeWebApp.ServiceProvider;
                        var medaitorClient = scopedProviderWebApp.GetRequiredService<IMediatorClientFactory>().GetMedaitorClient();

                        var request = new TestRequest1() { A = 6, B = 7 };
                        connectedClient = await medaitorClient.ConnectAsync(
                            ActivityId.NewId(),
                            request, 
                            null, 
                            CancellationToken.None);
                        Assert.NotNull(connectedClient);
                        testConnectedClient = (MediatorClientConnected<TestRequest1, TestResponse1>)connectedClient;
                        Assert.False(testConnectedClient.IsDisposed());
                    }
                }
            }
            Assert.True(testConnectedClient.IsDisposed());
        }

        [Fact]
        public async Task Medaitor_4WaitForAsync() {
            var servicesWebApp = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var servicesMediator = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            servicesWebApp.AddLatransMedaitor()
                .AddActivityHandler<TestActivityHandler1>()
                .AddActivityHandler<TestActivityHandler2>()
                .Build();

            ActivityId activityId = ActivityId.NewId();
            using (var serviceProviderMediator = servicesMediator.BuildServiceProvider()) {
                using (var serviceProviderWebApp = servicesWebApp.BuildServiceProvider()) {
                    using (var scopeWebApp = serviceProviderWebApp.CreateScope()) {
                        var scopedProviderWebApp = scopeWebApp.ServiceProvider;
                        var medaitorClient = scopedProviderWebApp.GetRequiredService<IMediatorClientFactory>().GetMedaitorClient();

                        var activityExecutionConfiguration = scopedProviderWebApp.GetRequiredService<ActivityExecutionConfigurationDefaults>().ForQueryCancelable;
                        var request = new TestRequest1() { A = 6, B = 7 };
                        var connectedClient = await medaitorClient.ConnectAsync(
                            activityId,
                            request,
                            activityExecutionConfiguration, 
                            CancellationToken.None);
                        var activityResponse = await connectedClient.WaitForAsync(activityExecutionConfiguration, CancellationToken.None);
                        Assert.NotNull(activityResponse);
                        Assert.NotNull(activityResponse as OkResultActivityResponse<TestResponse1>);
                        Assert.Equal(6*7+1,  ((OkResultActivityResponse<TestResponse1>)activityResponse).Result.R);
                    }
                }
            }
        }
        public class TestRequest1 : IRequest<TestResponse1> {
            public int A { get; set; }
            public int B { get; set; }
        }
        public class TestResponse1 : IResponseBase {
            public int R { get; set; }
        }

        public class TestActivityHandler1 : ActivityHandlerBase<TestRequest1, TestResponse1> {
            public override async Task ExecuteAsync(
                IActivityContext<TestRequest1, TestResponse1> activityContext,
                CancellationToken cancellationToken) {
                var request = activityContext.Request;
                var request2 = new TestRequest2() { A = request.A, B = request.B };
                ActivityId activityId = ActivityId.NewId();
                using var connectedClient = await activityContext.ConnectAsync(
                            activityId,
                            request2,
                            null, // activityExecutionConfiguration,
                            cancellationToken);
                var response2 = await connectedClient.WaitForAsync(null, cancellationToken);
                if (response2.TryGetResult<TestResponse2>(out var result2)) {                     
                    var response = new TestResponse1() { R = result2.R + 1 };
                    await activityContext.SetResponse(response);
                    return;
                } else {
                    await activityContext.SetActivityResponse(response2);
                }
            }
        }
        public class TestRequest2 : IRequest<TestResponse2> {
            public int A { get; set; }
            public int B { get; set; }
        }
        public class TestResponse2 : IResponseBase {
            public int R { get; set; }
        }

        public class TestActivityHandler2 : ActivityHandlerBase<TestRequest2, TestResponse2> {
            public override Task ExecuteAsync(
                IActivityContext<TestRequest2, TestResponse2> activityContext,
                CancellationToken cancellationToken) {
                var request = activityContext.Request;
                //activityContext.
                var response = new TestResponse2() { R = request.A * request.B };
                activityContext.SetResponse(response);
                return Task.CompletedTask;
            }
        }
    }
}
