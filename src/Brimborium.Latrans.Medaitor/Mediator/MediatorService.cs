﻿using System;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.Latrans.Activity;

using Microsoft.Extensions.DependencyInjection;

namespace Brimborium.Latrans.Mediator {
    public class MediatorService : IMediatorService, IDisposable {
        public static MediatorService Create(MediatorOptions options) {
            var result = new MediatorService(options ?? throw new ArgumentNullException(nameof(options)));
            result.Initialize();
            return result;
        }

        private ServiceProvider _ServicesMediator;
        private int _IsDisposed;

        public RequestRelatedTypes RequestRelatedTypes { get; }
        public ServiceProvider ServicesMediator { get => this._ServicesMediator; }

        public MediatorService(MediatorOptions options) {
            if (options is null) {
                throw new ArgumentNullException(nameof(options));
            }

            this.RequestRelatedTypes = new RequestRelatedTypes(options.RequestRelatedTypes.Items);
            this._ServicesMediator = options.ServicesMediator.BuildServiceProvider();
        }

        private void Initialize() {

        }

        protected virtual void Dispose(bool disposing) {
            if (0 == System.Threading.Interlocked.Exchange(ref this._IsDisposed, 1)) {
                using (var services = this._ServicesMediator) {
                    if (disposing) {
                        this._ServicesMediator = null;
                    }
                }
            }
        }

        ~MediatorService() {
            Dispose(disposing: false);
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public IActivityContext<TRequest> CreateContextByRequest<TRequest>(
            IMediatorClient medaitorClient,
            TRequest request
            ) {
            if (request is null) { throw new ArgumentNullException(nameof(request)); }
            //
            if (this.RequestRelatedTypes.Items.TryGetValue(typeof(TRequest), out var rrt)) {
                var result = rrt.CreateActivityContext(
                    new CreateActivityContextArguments() {
                        //ServiceProvider = this._ServicesMediator
                        MedaitorService = this
                    },
                    request);
                return (IActivityContext<TRequest>)result;
            } else {
                throw new NotSupportedException($"Unknown RequestType: {typeof(TRequest).FullName}");
            }
        }

        public Task SendAsync(
            IMediatorClient medaitorClient,
            IActivityContext activityContext,
            CancellationToken cancellationToken
            ) {
            var requestType = activityContext.GetRequestType();
            if (this.RequestRelatedTypes.Items.TryGetValue(requestType, out var rrt)) {
                Type handlerType = null;
                if (rrt.HandlerTypes.Length == 1) {
                    handlerType = rrt.HandlerTypes[0];
                }
                if (handlerType is object) {
                    var handler = (IActivityHandler)this._ServicesMediator.GetRequiredService(handlerType);
                    return handler.SendAsync(activityContext, cancellationToken);
                } else {
                    throw new NotSupportedException($"Invalid HandlerType for RequestType: {requestType.FullName}");
                }
            } else {
                throw new NotSupportedException($"Unknown RequestType: {requestType.FullName}");
            }
        }

        public async Task<IActivityResponse> WaitForAsync(
            IMediatorClient medaitorClient,
            IActivityContext activityContext,
            ActivityWaitForSpecification waitForSpecification,
            CancellationToken cancellationToken
            ) {
            return await activityContext.GetActivityResponseAsync();
        }

        public async Task<IMediatorClientConnected<TRequest>> ConnectAsync<TRequest>(
            IMediatorClient medaitorClient,
            TRequest request,
            CancellationToken cancellationToken) {
            if (request is null) { throw new ArgumentNullException(nameof(request)); }
            //
            if (this.RequestRelatedTypes.Items.TryGetValue(typeof(TRequest), out var rrt)) {
                var result = (IMediatorClientConnectedInternal<TRequest>)rrt.CreateClientConnected(
                    new CreateClientConnectedArguments() {
                        MedaitorService = this,
                        RequestRelatedType = rrt
                    },
                    request);
                return await result.SendAsync(cancellationToken);
            } else {
                throw new NotSupportedException($"Unknown RequestType: {typeof(TRequest).FullName}");
            }
        }

        //public IActivityContext<TRequest, TResponse> CreateContext<TRequest, TResponse>(
        //        IRequestRelatedType requestRelatedType,
        //        TRequest request
        //    ) {
        //    var result = ((RequestRelatedType)requestRelatedType).CreateActivityContext(
        //        new CreateActivityContextArguments() {
        //                //ServiceProvider = this._ServicesMediator
        //                MedaitorService = this
        //        },
        //        request);
        //    return (IActivityContext<TRequest, TResponse>)result;
        //}

        public IActivityHandler<TRequest, TResponse> CreateHandler<TRequest, TResponse>(
            IRequestRelatedType requestRelatedType,
            IActivityContext<TRequest, TResponse> activityContext) {
            IActivityHandler<TRequest, TResponse> result = null;
            if (requestRelatedType.DispatcherType is object) {
                var dispatchActivityHandler = (IDispatchActivityHandler<TRequest, TResponse>)this._ServicesMediator.GetService(requestRelatedType.DispatcherType);
                if (dispatchActivityHandler is null) {
                    throw new NotSupportedException($"Unknown IDispatchActivityHandler { requestRelatedType.DispatcherType.FullName } RequestType: {typeof(TRequest).FullName} ResponseType: {typeof(TResponse).FullName}");
                }
                result = dispatchActivityHandler.GetActivityHandler(
                    requestRelatedType.HandlerTypes,
                    activityContext,
                    (Type activityHandlerType) => (IActivityHandler<TRequest, TResponse>)this._ServicesMediator.GetService(activityHandlerType)
                    );
            } else if (requestRelatedType.HandlerTypes.Length == 1) {
                result = (IActivityHandler<TRequest, TResponse>)this._ServicesMediator.GetRequiredService(requestRelatedType.HandlerTypes[0]);
            } else {
                result = this._ServicesMediator.GetService<IActivityHandler<TRequest, TResponse>>();
            }
            if (result is null) {
                throw new NotSupportedException($"Unknown IActivityHandler RequestType: {typeof(TRequest).FullName} ResponseType: {typeof(TResponse).FullName}");
            } else {
                return result;
            }
        }
    }
}
