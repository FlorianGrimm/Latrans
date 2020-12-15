using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Medaitor;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Medaitor {
    public class MedaitorClient : IMedaitorClient, IDisposable {
        private bool _DisposedValue;

        public MedaitorClient() {

        }
        
        protected virtual void Dispose(bool disposing) {
            if (!_DisposedValue) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _DisposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MedaitorClient()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public IActivityContext<TRequest> CreateContextByRequest<TRequest>(TRequest request) {
            throw new NotImplementedException();
        }

        public IActivityContext<TRequest, TResponse> CreateContextByTypes<TRequest, TResponse>(TRequest request) {
            throw new NotImplementedException();
        }

        public Task SendAsync(IActivityContext medaitorContext, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task WaitForAsync(IActivityContext medaitorContext, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
    }
}
