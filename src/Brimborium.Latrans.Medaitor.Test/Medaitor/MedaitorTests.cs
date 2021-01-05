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
                        connectedClient = await medaitorClient.ConnectAndSendAsync(
                            ActivityId.NewId(),
                            request,
                            null,
                            CancellationToken.None);
                        Assert.False(connectedClient.GetActivityContext().IsDisposed);
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
                        var connectedClient = await medaitorClient.ConnectAndSendAsync(
                            activityId,
                            request,
                            activityExecutionConfiguration,
                            CancellationToken.None);
                        var activityResponse = await connectedClient.WaitForAsync(activityExecutionConfiguration, CancellationToken.None);
                        var status = await connectedClient.GetStatusAsync();
                        Assert.Equal(ActivityStatus.Completed, status.Status);
                        Assert.NotNull(activityResponse);
                        Assert.NotNull(activityResponse as OkResultActivityResponse<TestResponse1>);
                        Assert.Equal(6 * 7 + 1, ((OkResultActivityResponse<TestResponse1>)activityResponse).Result.R);
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
                IActivityContext<TestRequest1> activityContext,
                CancellationToken cancellationToken) {
                var request = activityContext.Request;
                var request2 = new TestRequest2() { A = request.A, B = request.B };
                ActivityId activityId = ActivityId.NewId();

                using var connectedClient = await activityContext.ConnectAndSendAsync(
                            activityId,
                            request2,
                            null, // activityExecutionConfiguration,
                            cancellationToken);

                var response2 = await connectedClient.WaitForAsync(null, cancellationToken);
                if (response2.TryGetResult<TestResponse2>(out var result2)) {
                    var response = new TestResponse1() { R = result2.R + 1 };
                    await this.SetResponseAsync(activityContext, response);
                    return;
                } else {
                    await activityContext.SetActivityResponseAsync(response2);
                    return;
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
                IActivityContext<TestRequest2> activityContext,
                CancellationToken cancellationToken) {
                var request = activityContext.Request;
                //activityContext.
                var response = new TestResponse2() { R = request.A * request.B };
                //activityContext.SetResponseAsync(response);
                return this.SetResponseAsync(activityContext, response);
            }
        }

        [Fact]
        public async Task Medaitor_5Cancel() {
            var servicesWebApp = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var servicesMediator = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            servicesWebApp.AddLatransMedaitor()
                .AddActivityHandler<TestActivityHandler5>()
                .Build();

            ActivityId activityId = ActivityId.NewId();
            using (var serviceProviderMediator = servicesMediator.BuildServiceProvider()) {
                using (var serviceProviderWebApp = servicesWebApp.BuildServiceProvider()) {
                    using (var scopeWebApp1 = serviceProviderWebApp.CreateScope()) {
                        var scopedProviderWebApp1 = scopeWebApp1.ServiceProvider;
                        var medaitorClient1 = scopedProviderWebApp1.GetRequiredService<IMediatorClientFactory>().GetMedaitorClient();

                        using (var scopeWebApp2 = serviceProviderWebApp.CreateScope()) {
                            var scopedProviderWebApp2 = scopeWebApp2.ServiceProvider;
                            var medaitorClient2 = scopedProviderWebApp2.GetRequiredService<IMediatorClientFactory>().GetMedaitorClient();

                            var activityExecutionConfiguration1 = scopedProviderWebApp2.GetRequiredService<ActivityExecutionConfigurationDefaults>().ForQueryCancelable;
                            var request5 = new TestRequest5() { A = 6, B = 7 };
                            var cts = new CancellationTokenSource();
                            var taskConnectedClient1 = medaitorClient1.ConnectAndSendAsync(
                                activityId,
                                request5,
                                activityExecutionConfiguration1,
                                cts.Token);
                            //

                            var connectedClient2 = await medaitorClient2.ConnectAsync(activityId, CancellationToken.None);

                            var status = await connectedClient2.GetStatusAsync();
                            Assert.Equal(ActivityStatus.Running, status.Status);
                            cts.Cancel();
                            IMediatorClientConnected<TestRequest5> connectedClient1;
                            try {
                                connectedClient1 = await taskConnectedClient1;
                            } catch {
                                connectedClient1 = null;
                            }
                            //
                            if (connectedClient1 is null) {
                                //
                            } else {
                                var activityResponse = await connectedClient1.WaitForAsync(activityExecutionConfiguration1, CancellationToken.None);
                                Assert.NotNull(activityResponse);
                                Assert.NotNull(activityResponse as OkResultActivityResponse<TestResponse5>);
                                Assert.Equal(6 * 7 - 2, ((OkResultActivityResponse<TestResponse5>)activityResponse).Result.R);
                            }
                        }
                    }
                }
            }
        }

        public class TestRequest5 : IRequest<TestResponse5> {
            public int A { get; set; }
            public int B { get; set; }
        }
        public class TestResponse5 : IResponseBase {
            public int R { get; set; }
        }

        public class TestActivityHandler5 : ActivityHandlerBase<TestRequest5, TestResponse5> {
            public override async Task ExecuteAsync(
                IActivityContext<TestRequest5> activityContext,
                CancellationToken cancellationToken) {
                var request = activityContext.Request;
                try {
                    await Task.Delay(TimeSpan.FromSeconds(50), cancellationToken);
                } catch {
                    var response = new TestResponse5() { R = request.A * request.B - 2};
                    await this.SetResponseAsync(activityContext, response);
                    return;
                }
                { 
                    var response = new TestResponse5() { R = request.A * request.B + 2};
                    await this.SetResponseAsync(activityContext, response);
                    return;
                }
            }
        }



        [Fact]
        public async Task Medaitor_6Cancel() {
            var servicesWebApp = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var servicesMediator = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            servicesWebApp.AddLatransMedaitor()
                .AddActivityHandler<TestActivityHandler6>()
                .Build();

            ActivityId activityId = ActivityId.NewId();
            using (var serviceProviderMediator = servicesMediator.BuildServiceProvider()) {
                using (var serviceProviderWebApp = servicesWebApp.BuildServiceProvider()) {
                    using (var scopeWebApp1 = serviceProviderWebApp.CreateScope()) {
                        var scopedProviderWebApp1 = scopeWebApp1.ServiceProvider;
                        var medaitorClient1 = scopedProviderWebApp1.GetRequiredService<IMediatorClientFactory>().GetMedaitorClient();

                        using (var scopeWebApp2 = serviceProviderWebApp.CreateScope()) {
                            var scopedProviderWebApp2 = scopeWebApp2.ServiceProvider;
                            var medaitorClient2 = scopedProviderWebApp2.GetRequiredService<IMediatorClientFactory>().GetMedaitorClient();

                            var activityExecutionConfiguration1 = scopedProviderWebApp2.GetRequiredService<ActivityExecutionConfigurationDefaults>().ForQueryCancelable;
                            var request6 = new TestRequest6() { A = 6, B = 7 };
                            var cts = new CancellationTokenSource();
                            var taskConnectedClient1 = medaitorClient1.ConnectAndSendAsync(
                                activityId,
                                request6,
                                activityExecutionConfiguration1,
                                cts.Token);
                            //

                            var connectedClient2 = await medaitorClient2.ConnectAsync(activityId, CancellationToken.None);

                            var status = await connectedClient2.GetStatusAsync();
                            Assert.Equal(ActivityStatus.Running, status.Status);
                            cts.Cancel();
                            IMediatorClientConnected<TestRequest6> connectedClient1;
                            try {
                                connectedClient1 = await taskConnectedClient1;
                            } catch {
                                connectedClient1 = null;
                            }
                            //
                            if (connectedClient1 is null) {
                                //
                            } else {
                                var activityResponse = await connectedClient1.WaitForAsync(activityExecutionConfiguration1, CancellationToken.None);
                                Assert.NotNull(activityResponse);
                                Assert.True(activityResponse is CanceledActivityResponse);
                            }
                        }
                    }
                }
            }
        }


        public class TestRequest6 : IRequest<TestResponse6> {
            public int A { get; set; }
            public int B { get; set; }
        }
        public class TestResponse6 : IResponseBase {
            public int R { get; set; }
        }

        public class TestActivityHandler6 : ActivityHandlerBase<TestRequest6, TestResponse6> {
            public override async Task ExecuteAsync(
                IActivityContext<TestRequest6> activityContext,
                CancellationToken cancellationToken) {
                var request = activityContext.Request;
                await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken);
                {
                    var response = new TestResponse6() { R = request.A * request.B };
                    await this.SetResponseAsync(activityContext, response);
                    return;
                }
            }
        }


#if false
        [Fact]
        public async Task Medaitor_9Monitor() {
            var servicesWebApp = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var servicesMediator = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            servicesWebApp.AddLatransMedaitor()
                .AddActivityHandler<TestActivityHandler5>()
                .Build();

            ActivityId activityId = ActivityId.NewId();
            using (var serviceProviderMediator = servicesMediator.BuildServiceProvider()) {
                using (var serviceProviderWebApp = servicesWebApp.BuildServiceProvider()) {
                    using (var scopeWebApp2 = serviceProviderWebApp.CreateScope()) {
                        var scopedProviderWebApp2 = scopeWebApp2.ServiceProvider;
                        var medaitorClient2 = scopedProviderWebApp2.GetRequiredService<IMediatorClientFactory>().GetMedaitorClient();


                        using (var scopeWebApp1 = serviceProviderWebApp.CreateScope()) {
                            var scopedProviderWebApp1 = scopeWebApp1.ServiceProvider;
                            var medaitorClient1 = scopedProviderWebApp1.GetRequiredService<IMediatorClientFactory>().GetMedaitorClient();

                            var activityExecutionConfiguration1 = scopedProviderWebApp1.GetRequiredService<ActivityExecutionConfigurationDefaults>().ForQueryCancelable;
                            var request5 = new TestRequest5() { A = 6, B = 7 };
                            var connectedClient = await medaitorClient1.ConnectAsync(
                                activityId,
                                request5,
                                activityExecutionConfiguration1,
                                CancellationToken.None);
                            //

                            //medaitorClient2.
                            //
                            var activityResponse = await connectedClient.WaitForAsync(activityExecutionConfiguration1, CancellationToken.None);
                            Assert.NotNull(activityResponse);
                            Assert.NotNull(activityResponse as OkResultActivityResponse<TestResponse5>);
                            Assert.Equal(6 * 7 + 1, ((OkResultActivityResponse<TestResponse5>)activityResponse).Result.R);
                        }
                    }
                }
            }
        }
#endif

        /*
        public class TestRequest4 : IRequest<TestResponse4> {
            public int A { get; set; }
            public int B { get; set; }
        }
        public class TestResponse4 : IResponseBase {
            public int R { get; set; }
        }

        public class TestActivityHandler4 : ActivityHandlerBase<TestRequest4, TestResponse4> {
            public override Task ExecuteAsync(
                IActivityContext<TestRequest4, TestResponse4> activityContext,
                CancellationToken cancellationToken) {
                var request = activityContext.Request;
                //activityContext.
                var response = new TestResponse4() { R = request.A * request.B };
                activityContext.SetResponse(response);
                return Task.CompletedTask;
            }
        }

        */

#if false
        public class TestRequestXXX : IRequest<TestResponseXXX> {
            public int A { get; set; }
            public int B { get; set; }
        }
        public class TestResponseXXX : IResponseBase {
            public int R { get; set; }
        }

        public class TestActivityHandlerXXX : ActivityHandlerBase<TestRequestXXX, TestResponseXXX> {
            public override Task ExecuteAsync(
                IActivityContext<TestRequestXXX, TestResponseXXX> activityContext,
                CancellationToken cancellationToken) {
                var request = activityContext.Request;
                //activityContext.
                var response = new TestResponseXXX() { R = request.A * request.B };
                activityContext.SetResponse(response);
                return Task.CompletedTask;
            }
        }
#endif
    }
}
