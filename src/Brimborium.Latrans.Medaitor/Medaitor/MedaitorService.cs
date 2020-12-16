using System;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.Latrans.Activity;

using Microsoft.Extensions.DependencyInjection;

namespace Brimborium.Latrans.Medaitor {
    public class MedaitorService : IMedaitorService, IDisposable {
        public static MedaitorService Create(MediatorOptions options) {
            var result = new MedaitorService(options ?? throw new ArgumentNullException(nameof(options)));
            result.Initialize();
            return result;
        }

        private ServiceProvider _ServicesMediator;
        private int _IsDisposed;

        public RequestRelatedTypes RequestRelatedTypes { get; }
        public ServiceProvider ServicesMediator { get => this._ServicesMediator; }

        public MedaitorService(MediatorOptions options) {
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

        ~MedaitorService() {
            Dispose(disposing: false);
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public IActivityContext<TRequest> CreateContextByRequest<TRequest>(
            IMedaitorClient medaitorClient,
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
            IMedaitorClient medaitorClient,
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
            IMedaitorClient medaitorClient,
            IActivityContext activityContext,
            ActivityWaitForSpecification waitForSpecification,
            CancellationToken cancellationToken
            ) {
            return await activityContext.GetActivityResponseAsync();
        }

        public async Task<IMedaitorClientConnected<TRequest>> ConnectAsync<TRequest>(
            IMedaitorClient medaitorClient,
            TRequest request) {
            if (request is null) { throw new ArgumentNullException(nameof(request)); }
            //
            if (this.RequestRelatedTypes.Items.TryGetValue(typeof(TRequest), out var rrt)) {
                var result = (IMedaitorClientConnectedInternal<TRequest>)rrt.CreateClientConnected(
                    new CreateClientConnectedArguments() {
                        //ServiceProvider = this._ServicesMediator
                        MedaitorService = this
                    },
                    request);
                return await result.SendAsync();
            } else {
                throw new NotSupportedException($"Unknown RequestType: {typeof(TRequest).FullName}");
            }
        }
    }
}
