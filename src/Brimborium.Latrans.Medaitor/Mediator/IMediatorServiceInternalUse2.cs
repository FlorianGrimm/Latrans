
using Brimborium.Latrans.Activity;

using System;

namespace Brimborium.Latrans.Mediator {
    /// <summary>
    /// public accessor - but internal use.
    /// </summary>
    public interface IMediatorServiceInternalUse2 : IMediatorServiceInternalUse {
        IActivityContext<TRequest, TResponse> CreateContext<TRequest, TResponse>(
                RequestRelatedType? requestRelatedType,
                TRequest request
            );


        void HandleRequestForAccepted202Redirect<TRequest, TResponse>(
                IActivityContext<TRequest, TResponse> activityContext
            );

        void HandleRequestAfterTimeout<TRequest, TResponse>(
                IActivityContext<TRequest, TResponse> activityContext
            );

    }

    public interface IMediatorScopeServiceInternalUse : IMediatorScopeService {
        
        IActivityHandler<TRequest, TResponse> CreateHandler<TRequest, TResponse>(
                RequestRelatedType? requestRelatedType,
                IActivityContext<TRequest, TResponse> activityContext
            );
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
