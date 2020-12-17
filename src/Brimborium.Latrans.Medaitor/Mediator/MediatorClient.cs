using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Mediator;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {
    public class MediatorClient : IMediatorClient, IDisposable {
        private int _IsDisposed;
        private readonly IMediatorService _MedaitorService;

        public MediatorClient(IMediatorService medaitorService) {
            this._MedaitorService = medaitorService ?? throw new ArgumentNullException(nameof(medaitorService));
        }

        public bool IsDisposed => (this._IsDisposed == 1);

        protected virtual void Dispose(bool disposing) {
            if (0 == System.Threading.Interlocked.Exchange(ref this._IsDisposed, 1)) {
                if (disposing) {
                }
            }
        }

        ~MediatorClient() {
            Dispose(disposing: false);
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public IActivityContext<TRequest> CreateContextByRequest<TRequest>(TRequest request) {
            return this._MedaitorService.CreateContextByRequest<TRequest>(this, request);
        }

        public Task SendAsync(
            IActivityContext activityContext,
            CancellationToken cancellationToken) {
            return this._MedaitorService.SendAsync(this, activityContext, cancellationToken);
        }

        public Task WaitForAsync(
            IActivityContext activityContext,
            ActivityWaitForSpecification waitForSpecification,
            CancellationToken cancellationToken) {
            return this._MedaitorService.WaitForAsync(this, activityContext, waitForSpecification, cancellationToken);
        }

        public async Task<IMediatorClientConnected<TRequest>> ConnectAsync<TRequest>(TRequest request, CancellationToken cancellationToken) {
            var result = await this._MedaitorService.ConnectAsync<TRequest>(this, request, cancellationToken);
#warning TODO
            return result;
        }
    }

}
