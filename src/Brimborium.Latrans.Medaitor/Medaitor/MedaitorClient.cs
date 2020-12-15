using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Medaitor;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Medaitor {
    public class MedaitorClient : IMedaitorClient, IDisposable {
        private int _IsDisposed;
        private readonly IMedaitorService _MedaitorService;

        public MedaitorClient(IMedaitorService medaitorService) {
            this._MedaitorService = medaitorService ?? throw new ArgumentNullException(nameof(medaitorService));
        }

        public bool IsDisposed => (this._IsDisposed == 1);

        protected virtual void Dispose(bool disposing) {
            if (0 == System.Threading.Interlocked.Exchange(ref this._IsDisposed, 1)) {
                if (disposing) {
                }
            }
        }

        ~MedaitorClient() {
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
            throw new NotImplementedException();
        }
    }
}
