﻿using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Mediator;
using Brimborium.Latrans.Utility;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {
    public class MediatorClient
        : IMediatorClient
        , IDisposable {
        private readonly IMediatorService _MedaitorService;
        private int _IsDisposed;
        private IMediatorClientConnected? _MediatorClientConnected;

        // ScopedServiceProvider Web
        private readonly IServiceProvider? _ServiceProvider;

        // ScopedServiceProvider Web
        private readonly ILocalDisposables _LocalDisposables;
        private readonly bool _DisposeLocalDisposables;

        public MediatorClient(
            IMediatorService medaitorService
            ) {
            this._MedaitorService = medaitorService ?? throw new ArgumentNullException(nameof(medaitorService));
            this._LocalDisposables = new LocalDisposables();
            this._DisposeLocalDisposables = true;
        }

        public MediatorClient(
            IMediatorService medaitorService,
            IServiceProvider serviceProvider,
            ILocalDisposables localDisposables
            ) {
            this._MedaitorService = medaitorService ?? throw new ArgumentNullException(nameof(medaitorService));
            this._ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this._LocalDisposables = localDisposables ?? throw new ArgumentNullException(nameof(localDisposables));
            this._DisposeLocalDisposables = false;
        }


        public bool IsDisposed => (this._IsDisposed == 1);

        private void Dispose(bool disposing) {
            if (0 == System.Threading.Interlocked.Exchange(ref this._IsDisposed, 1)) {
                if (this._DisposeLocalDisposables) {
                    this._LocalDisposables.Dispose();
                }
            }
        }

        ~MediatorClient() {
            this.Dispose(disposing: false);
        }

        public void Dispose() {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task<IMediatorClientConnected<TRequest>> ConnectAndSendAsync<TRequest>(
            ActivityId activityId,
            TRequest request,
            ActivityExecutionConfiguration activityExecutionConfiguration,
            CancellationToken cancellationToken) {
            var result = await this._MedaitorService.ConnectAsync<TRequest>(
                this,
                activityId,
                request,
                activityExecutionConfiguration,
                cancellationToken);
            this._LocalDisposables.Add(result);
            this._MediatorClientConnected = result;
            await result.SendAsync(cancellationToken);
            return result;
        }

        public Task<IMediatorClientConnected<TRequest>> ConnectAsync<TRequest>(ActivityId activityId, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public async Task<MediatorActivityStatus> GetStatusAsync() {
            var activityContext = this._MediatorClientConnected?.GetActivityContext();
            if (activityContext is object) {
                return await activityContext.GetStatusAsync();
            } else {
                return new MediatorActivityStatus() {
                    Status = ActivityStatus.Unknown
                };
            }
        }

#if false
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
#endif
    }
}
