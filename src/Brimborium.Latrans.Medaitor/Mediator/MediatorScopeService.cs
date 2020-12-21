using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Utility;

using Microsoft.Extensions.DependencyInjection;

using System;
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

        public MediatorScopeService(
            MediatorService mediatorService) {
            this._MediatorService = mediatorService;
            this._Scope = mediatorService.ServicesMediator.CreateScope();
            this._ServiceProvider = this._Scope.ServiceProvider;
        }

        protected virtual void Dispose(bool disposing) {
            if (0 == System.Threading.Interlocked.Exchange(ref this._IsDisposed, 1)) {
                using (var scope = this._Scope) {
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

        public void AddClientConnected<TRequest>(
            IMediatorClientConnected<TRequest> mediatorClientConnected
            ) {
        }

        public IActivityHandler<TRequest, TResponse> CreateHandler<TRequest, TResponse>(
           RequestRelatedType? requestRelatedType,
           IActivityContext<TRequest, TResponse> activityContext) {
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
