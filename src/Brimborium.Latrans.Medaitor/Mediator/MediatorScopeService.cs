using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Collections;
using Brimborium.Latrans.Utility;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {
    public class MediatorScopeService
        : IMediatorScopeService
        , IMediatorScopeServiceInternalUse
        //, IMediatorServiceInternal
        , IDisposable {
        private MediatorService _MediatorService;
        private IServiceScope _Scope;
        private IServiceProvider _ServiceProvider;
        private int _IsDisposed;
        private readonly List<IMediatorClientConnected> _MediatorClientConnecteds;
        private AtomicReference<ImList<Action<MediatorScopeService>>> _ActionDisposing;

        public MediatorScopeService(
                MediatorService mediatorService
            ) {
            this._ActionDisposing = new AtomicReference<ImList<Action<MediatorScopeService>>>(ImList<Action<MediatorScopeService>>.Empty);
            this._Scope = mediatorService.ServicesMediator.CreateScope();
            this._ServiceProvider = this._Scope.ServiceProvider;
            this._MediatorClientConnecteds = new List<IMediatorClientConnected>();
            this._MediatorService = mediatorService;
        }

        protected virtual void Dispose(bool disposing) {
            if (0 == System.Threading.Interlocked.Exchange(ref this._IsDisposed, 1)) {
                using (var scope = this._Scope) {
                    var arrayActionDisposing = this._ActionDisposing.SetValue(ImList<Action<MediatorScopeService>>.Empty).ToArray();
                    foreach (var actionDisposing in arrayActionDisposing) {
                        if (actionDisposing is null) {
                        } else {
                            actionDisposing(this);
                        }
                    }
                    if (disposing) {
                        this._ServiceProvider = DisposedServiceProvider.Instance;
                        this._Scope = DisposedServiceScope.Instance;
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

        public IServiceProvider ServiceProvider => this._ServiceProvider;

        public IMediatorServiceStorage Storage => this._MediatorService.Storage;

        public bool AddClientConnected<TRequest>(
            IMediatorClientConnected<TRequest> mediatorClientConnected
            ) {
            bool result = true;
            lock (this._MediatorClientConnecteds) {
                foreach (var mcc in this._MediatorClientConnecteds) {
                    if (ReferenceEquals(mcc, mediatorClientConnected)) {
                        result = false;
                    }
                }
                if (result) {
                    this._MediatorClientConnecteds.Add(mediatorClientConnected);
                }
            }
            return result;
        }

        public bool RemoveClientConnected<TRequest>(IMediatorClientConnected<TRequest> mediatorClientConnected) {
            bool result = false;
            lock (this._MediatorClientConnecteds) {
                result = this._MediatorClientConnecteds.Remove(mediatorClientConnected);
            }
            return result;
        }

        public IActivityHandler<TRequest, TResponse> CreateHandler<TRequest, TResponse>(
                RequestRelatedType? requestRelatedType,
                IActivityContext<TRequest> activityContext
            )
            where TRequest : IRequest<TResponse>, IRequestBase
            where TResponse : IResponseBase {
            if (requestRelatedType is null) {
                if (!this._MediatorService.TryRequestRelatedType(typeof(TRequest), out requestRelatedType)
                    || (requestRelatedType is null)) {
                    throw new NotSupportedException($"Unknown RequestType: {typeof(TRequest).FullName}");
                }
            }

            if (activityContext is null) {
                throw new ArgumentNullException(nameof(activityContext));
            }

            IActivityHandler<TRequest, TResponse>? result = null;
            if (requestRelatedType.DispatcherType is object) {
                var dispatchActivityHandler = (IDispatchActivityHandler<TRequest, TResponse>)this._ServiceProvider.GetService(requestRelatedType.DispatcherType);
                if (dispatchActivityHandler is null) {
                    throw new NotSupportedException($"Unknown IDispatchActivityHandler { requestRelatedType.DispatcherType.FullName } RequestType: {typeof(TRequest).FullName} ResponseType: {typeof(TResponse).FullName}");
                }
                result = dispatchActivityHandler.GetActivityHandler(
                    requestRelatedType.HandlerTypes,
                    activityContext,
                    (Type activityHandlerType) => (IActivityHandler<TRequest, TResponse>)this._ServiceProvider.GetService(activityHandlerType)
                    );
            } else if (requestRelatedType.HandlerTypes.Length == 1) {
                result = (IActivityHandler<TRequest, TResponse>)this._ServiceProvider.GetRequiredService(requestRelatedType.HandlerTypes[0]);
            } else {
                result = this._ServiceProvider.GetService<IActivityHandler<TRequest, TResponse>>();
            }
            if (result is null) {
                throw new NotSupportedException($"Unknown IActivityHandler RequestType: {typeof(TRequest).FullName} ResponseType: {typeof(TResponse).FullName}");
            } else {
                return result;
            }
        }

        
        public Task<IMediatorClientConnected<TRequest>> ConnectAsync<TRequest>(
            ActivityId activityId,
            TRequest request,
            ActivityExecutionConfiguration activityExecutionConfiguration,
            CancellationToken cancellationToken) {
            try {
                if (request is null) { throw new ArgumentNullException(nameof(request)); }
                //
                if (this._MediatorService.TryRequestRelatedType(typeof(TRequest), out var rrt)) {
                    var mediatorClientConnected
                        = (IMediatorClientConnected<TRequest>)rrt.FactoryClientConnected(
                            this._ServiceProvider,
                            new object[] {
                            new CreateClientConnectedArguments(
                                this._MediatorService,
                                this,
                                activityId,
                                rrt),
                            request });
                    mediatorClientConnected.Initialize();
                    return Task.FromResult<IMediatorClientConnected<TRequest>>(mediatorClientConnected);
                } else {
                    throw new NotSupportedException($"Unknown RequestType: {typeof(TRequest).FullName}");
                }
            } catch (System.Exception error) {
                return Task.FromException<IMediatorClientConnected<TRequest>>(error);
            }
        }

        internal void AddDisposing(Action<MediatorScopeService> removeMediatorScopeService) {
            this._ActionDisposing.Mutate1(removeMediatorScopeService, (a, l) => l.Add(a));
        }

        internal bool TryGetMediatorClientConnected(ActivityId activityId, [MaybeNullWhen(false)] out IMediatorClientConnected result) {
            lock (this._MediatorClientConnecteds) {
                for (int idx = 0; idx < this._MediatorClientConnecteds.Count; idx++) {
                    var item = this._MediatorClientConnecteds[idx];
                    var activityContext = item.GetActivityContext();
                    if (activityContext is object && activityContext.ActivityId.Equals(activityId)) {
                        result = item;
#warning todo
                        return true;
                    }
                }
            }
            {
                result = null;
                return false;
            }
        }


        //public Task<IMediatorClientConnected<TRequest>> ConnectAsync<TRequest>(IMediatorClient medaitorClient, TRequest request, CancellationToken cancellationToken) {
        //    throw new System.NotImplementedException();
        //}

        //public IActivityContext<TRequest, TResponse> CreateContext<TRequest, TResponse>(RequestRelatedType? requestRelatedType, TRequest request) {
        //    throw new System.NotImplementedException();
        //}

        //public IActivityHandler<TRequest, TResponse> CreateHandler<TRequest, TResponse>(RequestRelatedType? requestRelatedType, IActivityContext<TRequest, TResponse> activityContext) {
        //    throw new System.NotImplementedException();
        //}

        //public void HandleRequestAfterTimeout<TRequest, TResponse>(IActivityContext<TRequest, TResponse> activityContext) {
        //    throw new System.NotImplementedException();
        //}

        //public void HandleRequestForAccepted202Redirect<TRequest, TResponse>(IActivityContext<TRequest, TResponse> activityContext) {
        //    throw new System.NotImplementedException();
        //}
    }
#if false
    public class MediatorScopeService
        : IMediatorScopeService
        , IMediatorServiceInternal
        , IDisposable {
        private Task _TimeoutTasks;
        private ServiceProvider _ServicesMediator;
        private readonly IMediatorServiceStorage _Storage;

        public MediatorScopeService() {
        }

        public IMediatorServiceStorage Storage => throw new NotImplementedException();

      
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
