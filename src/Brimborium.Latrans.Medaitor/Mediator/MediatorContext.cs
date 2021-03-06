﻿using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Collections;
using Brimborium.Latrans.Utility;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {
    public class MediatorContext<TRequest>
        : IActivityContext<TRequest>
        , IDisposable {
        private IMediatorService _MedaitorService;
        private MediatorScopeService _MediatorScopeService;
        private TRequest _Request;
        private IActivityResponse? _ActivityResponse;
        private int _IsDisposed;
        private readonly ActivityCompletion<IActivityResponse> _ActivityCompletion;
        private readonly AtomicReference<ImList<IActivityEvent>> _ActivityEvents;
        private readonly LocalDisposables _LocalDisposables;
        private ActivityId _ActivityId;
        //private Guid _OperationId;
        //private Guid _ExecutionId;
#if false
        public MediatorContext() {
            this._ActivityCompletion = new ActivityCompletion<IActivityResponse>();
            this._ActivityEvents = new AtomicReference<ImmutableList<IActivityEvent>>(
                    ImmutableList<IActivityEvent>.Empty
                );
        }
#endif

        [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor()]
        public MediatorContext(CreateActivityContextArguments arguments, TRequest request) {
            this._ActivityCompletion = new ActivityCompletion<IActivityResponse>();
            this._ActivityEvents = new AtomicReference<ImList<IActivityEvent>>(ImList<IActivityEvent>.Empty);
            this._Request = request;
            this._LocalDisposables = LocalDisposables.Create(this);
            //
            this.ActivityId = arguments.ActivityId;
            this._MedaitorService = arguments.MedaitorService;
            this._MediatorScopeService = arguments.MediatorScopeService;
            this.Status = ActivityStatus.Unknown;
        }

        public Type GetRequestType() => typeof(TRequest);

        public ActivityStatus Status { get; set; }

        public ActivityId ActivityId { 
            get {
                return this._ActivityId;
            }
            set {
                this._ActivityId = value;
            }
        }
        //public Guid OperationId {
        //    get {
        //        if (this._OperationId == Guid.Empty) {
        //            lock (this) {
        //                if (this._OperationId == Guid.Empty) {
        //                    this._OperationId = Guid.NewGuid();
        //                }
        //            }
        //        }
        //        return this._OperationId;
        //    }
        //    set {
        //        if (this.Status != ActivityStatus.Initialize && this._OperationId != Guid.Empty) {
        //            throw new NotSupportedException($"{nameof(OperationId)} is already set.");
        //        }
        //        this._OperationId = value;
        //    }
        //}

        //public Guid ExecutionId {
        //    get {
        //        if (this._ExecutionId == Guid.Empty) {
        //            lock (this) {
        //                if (this._ExecutionId == Guid.Empty) {
        //                    this._ExecutionId = Guid.NewGuid();
        //                }
        //            }
        //        }
        //        return this._ExecutionId;
        //    }
        //    set {
        //        if (this.Status != ActivityStatus.Initialize && this._ExecutionId != Guid.Empty) {
        //            throw new NotSupportedException($"{nameof(ExecutionId)} is already set.");
        //        }
        //        this._ExecutionId = value;
        //    }
        //}

        public TRequest Request {
            get { return this._Request; }
            set { this._Request = value; }
        }

        public IActivityEvent[] ActivityEvents {
            get {
                return this._ActivityEvents.Value.ToArray();
            }
            set {
                this._ActivityEvents.SetValue(new ImList<IActivityEvent>(value));
            }
        }

#if weichei
        public IMediatorService MedaitorService {
            get { return this._MedaitorService; }
            set {
                if (ReferenceEquals(this._MedaitorService, value)) { return; }
                if (this._MedaitorService is object) {
                    throw new ArgumentException("already set", nameof(this.MedaitorService));
                }
                if (value is null) {
                    throw new ArgumentNullException(nameof(this.MedaitorService));
                }
                this._MedaitorService = value;
            }
        }
#endif

        public IMediatorScopeService MediatorScopeService {
            get { return this._MediatorScopeService; }
            set {
                if (ReferenceEquals(this._MediatorScopeService, value)) { return; }
                if (this._MediatorScopeService is object) {
                    throw new ArgumentException("already set", nameof(this.MediatorScopeService));
                }
                if (value is null) {
                    throw new ArgumentNullException(nameof(this.MediatorScopeService));
                }
                this._MediatorScopeService = (MediatorScopeService)value;
            }
        }

        public bool IsDisposed => (this._IsDisposed != 0);

        public async Task AddActivityEventAsync(IActivityEvent activityEvent) {
            if (activityEvent is null) {
                return;
            } else {
                this._ActivityEvents.Mutate1<IActivityEvent>(
                    activityEvent,
                    (newItem, activityEvents) => {
                        newItem.SequenceNo = activityEvents.Count + 1;
                        return activityEvents.Add(newItem);
                    });
                await this._MediatorScopeService.Storage.AddActivityEventAsync(activityEvent);
                if (activityEvent is IActivityEventChangeStatus activityEventChangeStatus) {
                    this.Status = activityEventChangeStatus.Status;
                }
            }
        }

        public async Task SetActivityResponseAsync(IActivityResponse activityResponse) {
            if (ReferenceEquals(this._ActivityResponse, activityResponse)) { return; }

            if (activityResponse is null) {
                throw new ArgumentNullException(nameof(activityResponse));
            }

            var prev = System.Threading.Interlocked.CompareExchange(ref this._ActivityResponse, activityResponse, null);
            if (prev is null) {
                var activityEvent = activityResponse.GetAsActivityEvent(this);
                await this.AddActivityEventAsync(activityEvent);
                this._ActivityCompletion.TrySetResult(activityResponse);
            } else {
                throw new ArgumentException("already set", nameof(activityResponse));
            }
        }

        public Task SetFailureAsync(Exception error) {
#warning here SetFailureAsync
            return this.SetActivityResponseAsync(new FailureActivityResponse(error));
        }

#if weichei
        public Task SetResponseAsync(TResponse response) {
            return this.SetActivityResponseAsync(new OkResultActivityResponse<TResponse>(response));
        }
#endif

        public Task<IActivityResponse> GetActivityResponseAsync() {
            return this._ActivityCompletion.Task;
        }

        public async Task<IMediatorClientConnected<TRequestInner>> ConnectAndSendAsync<TRequestInner>(
                ActivityId activityId,
                TRequestInner request,
                ActivityExecutionConfiguration activityExecutionConfiguration,
                CancellationToken cancellationToken
            ) {
            var result = await this._MediatorScopeService.ConnectAsync(activityId, request, activityExecutionConfiguration, cancellationToken);
            this._LocalDisposables.Add(result);
            await result.SendAsync(cancellationToken);
            return result;
        }

        ~MediatorContext() {
            this.Dispose(disposing: false);
        }

        public void Dispose() {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (0 == System.Threading.Interlocked.Exchange(ref this._IsDisposed, 1)) {
                if (disposing) {
                    if (_ActivityCompletion.IsNotDefined) {
                        _ActivityCompletion.TrySetResult(new FailureActivityResponse(new Exception("Disposing")));
                    }
                } else {
                    if (_ActivityCompletion.IsNotDefined) {
                        _ActivityCompletion.TrySetResult(new FailureActivityResponse(new Exception("Disposed")));
                    }
                }
                this._ActivityCompletion.Dispose();
                this._LocalDisposables.Dispose();
            }
        }

        public Task<IMediatorClientConnected?> ConnectAsync(ActivityId activityId, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<MediatorActivityStatus> GetStatusAsync() {
            var result = new MediatorActivityStatus() { 
                Status = this.Status,
                ActivityId = this.ActivityId,
                ActivityEvents = this._ActivityEvents.Value.ToArray()
            };
            return Task<MediatorActivityStatus>.FromResult(result);
        }

    }
}
