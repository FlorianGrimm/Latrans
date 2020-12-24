using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Utility;

using Microsoft.Extensions.DependencyInjection;

namespace Brimborium.Latrans.Mediator {

    public class MediatorService
        : IMediatorService
        , IMediatorServiceInternalUse
        , IDisposable {

        public static MediatorService Create(MediatorOptions options) {
            var result = new MediatorService(options ?? throw new ArgumentNullException(nameof(options)));
            result.Initialize();
            return result;
        }

        private Task _TimeoutTasks;
        private IServiceProvider _ServicesMediator;
        private int _IsDisposed;
        private readonly IMediatorServiceStorage _Storage;
        private readonly List<MediatorScopeService> _MediatorScopeServices;

        public RequestRelatedTypes RequestRelatedTypes { get; }
        public IServiceProvider ServicesMediator { get => this._ServicesMediator; }

        public IMediatorServiceStorage Storage => this._Storage;

        public MediatorService(MediatorOptions options) {
            if (options is null) {
                throw new ArgumentNullException(nameof(options));
            }
            this.RequestRelatedTypes = new RequestRelatedTypes(options.RequestRelatedTypes.Items);
            this._ServicesMediator = options.ServicesMediator.BuildServiceProvider();
            this._TimeoutTasks = Task.CompletedTask;
            this._MediatorScopeServices = new List<MediatorScopeService>();
            this._Storage = new MediatorServiceStorageNull();
        }

        private void Initialize() {

        }

        protected virtual void Dispose(bool disposing) {
            if (0 == System.Threading.Interlocked.Exchange(ref this._IsDisposed, 1)) {
                using (var services = (this._ServicesMediator as IDisposable)) {
                    if (disposing) {
                        this._ServicesMediator = DisposedServiceProvider.Instance;
                    }
                }
            }
        }

        ~MediatorService() {
            Dispose(disposing: false);
        }

        public bool TryRequestRelatedType(Type type, [MaybeNullWhen(false)] out RequestRelatedType requestRelatedType)
            => this.RequestRelatedTypes.Items.TryGetValue(type, out requestRelatedType);

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public Task<IMediatorClientConnected<TRequest>> ConnectAsync<TRequest>(
            IMediatorClient medaitorClient,
            ActivityId activityId,
            TRequest request,
            ActivityExecutionConfiguration activityExecutionConfiguration,
            CancellationToken cancellationToken) {
            try {
                if (request is null) { throw new ArgumentNullException(nameof(request)); }
                //
                if (this.RequestRelatedTypes.Items.TryGetValue(typeof(TRequest), out var rrt)) {

                    var mediatorClientConnected
                        = (IMediatorClientConnected<TRequest>)rrt.FactoryClientConnected(
                            this._ServicesMediator,
                            new object[] {
                            new CreateClientConnectedArguments(
                                this,
                                medaitorClient,
                                activityId,
                                rrt),
                            request });
                    mediatorClientConnected.Initialize();
                    return Task<IMediatorClientConnected<TRequest>>.FromResult(mediatorClientConnected);
                } else {
                    throw new NotSupportedException($"Unknown RequestType: {typeof(TRequest).FullName}");
                }
            } catch (System.Exception error){
                return Task.FromException<IMediatorClientConnected<TRequest>>(error);
            }
        }

        public IActivityContext<TRequest, TResponse> CreateContext<TRequest, TResponse>(
                ActivityId activityId,
                TRequest request,
                RequestRelatedType? requestRelatedType
            ) {
            if (requestRelatedType is null) {
                if (!this.RequestRelatedTypes.TryGetValue(typeof(TRequest), out requestRelatedType)) {
                    throw new NotSupportedException($"Unknown RequestType: {typeof(TRequest).FullName}");
                }
            }
            var mediatorScopeService = this.CreateMediatorScopeService();
            var resultObj = requestRelatedType.FactoryActivityContext(
                mediatorScopeService.ServiceProvider,
                new object[] {
                    new CreateActivityContextArguments(
                        this,
                        mediatorScopeService,
                        activityId
                        ),
                    request! });
            if (resultObj is null) {
                throw new InvalidOperationException("FactoryActivityContext returns null.");
            }
            var result = (IActivityContext<TRequest, TResponse>)resultObj;
#warning TODO var x = result.MediatorScopeService;
            return result;
        }

        private MediatorScopeService CreateMediatorScopeService() {
            var mediatorScopeService = new MediatorScopeService(this);
            lock (this._MediatorScopeServices) {
                this._MediatorScopeServices.Add(mediatorScopeService);
            }
            return mediatorScopeService;
        }

#if false
        public IActivityHandler<TRequest, TResponse> CreateHandler<TRequest, TResponse>(
            RequestRelatedType? requestRelatedType,
            IActivityContext<TRequest, TResponse> activityContext) {
            if (requestRelatedType is null) {
                if (!this.RequestRelatedTypes.TryGetValue(typeof(TRequest), out requestRelatedType)) {
                    throw new NotSupportedException($"Unknown RequestType: {typeof(TRequest).FullName}");
                }
            }

            if (activityContext is null) {
                throw new ArgumentNullException(nameof(activityContext));
            }

            IActivityHandler<TRequest, TResponse>? result = null;
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
#endif

        public void HandleRequestForAccepted202Redirect<TRequest, TResponse>(IActivityContext<TRequest, TResponse> activityContext) {
            //activityContext.OperationId
            //activityContext.ExecutionId
            throw new NotImplementedException();
        }

        public void HandleRequestAfterTimeout<TRequest, TResponse>(IActivityContext<TRequest, TResponse> activityContext) {
            lock (this) {
                var taskRequest = activityContext.GetActivityResponseAsync();
                this._TimeoutTasks = this._TimeoutTasks.ContinueWith((previousTask) => {
                    return taskRequest.ContinueWith((t2) => {
                        if (t2.IsFaulted) {
                            this.LogException(t2.Exception);
                        }
                    });
                }).Unwrap();
            }
        }

        private void LogException(AggregateException? exception) {
            //throw new NotImplementedException();
        }

        public Task<MediatorActivityStatus[]> GetStatusAsync() {
            throw new NotImplementedException();
        }
    }
#if false
    public class MediatorScopeService
        : IMediatorScopeService
        , IMediatorServiceInternal
        , IDisposable {
        private Task _TimeoutTasks;
        private ServiceProvider _ServicesMediator;
        private int _IsDisposed;
        private readonly IMediatorServiceStorage _Storage;

        public MediatorScopeService() {
        }

        public IMediatorServiceStorage Storage => throw new NotImplementedException();

        protected virtual void Dispose(bool disposing) {
            if (0 == System.Threading.Interlocked.Exchange(ref this._IsDisposed, 1)) {
                using (var services = this._ServicesMediator) {
                    if (disposing) {
                        this._ServicesMediator = null;
                    }
                }
            }
        }

        ~MediatorScopeService() {
            Dispose(disposing: false);
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public IActivityContext<TRequest, TResponse> CreateContext<TRequest, TResponse>(RequestRelatedType? requestRelatedType, TRequest request) {
            throw new NotImplementedException();
        }

        public IActivityHandler<TRequest, TResponse> CreateHandler<TRequest, TResponse>(RequestRelatedType? requestRelatedType, IActivityContext<TRequest, TResponse> activityContext) {
            throw new NotImplementedException();
        }

        public void HandleRequestForAccepted202Redirect<TRequest, TResponse>(IActivityContext<TRequest, TResponse> activityContext) {
            throw new NotImplementedException();
        }

        public void HandleRequestAfterTimeout<TRequest, TResponse>(IActivityContext<TRequest, TResponse> activityContext) {
            throw new NotImplementedException();
        }

        public Task<IMediatorClientConnected<TRequest>> ConnectAsync<TRequest>(IMediatorClient medaitorClient, TRequest request, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
    }
#endif
}
