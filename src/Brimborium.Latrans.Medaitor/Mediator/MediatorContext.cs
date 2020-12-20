using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Utility;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {
    public class MediatorContext<TRequest, TResponse>
        : IActivityContext<TRequest, TResponse>
        , IDisposable {
        private IMediatorService _MedaitorService;
        private TRequest _Request;
        private IActivityResponse? _ActivityResponse;
        private int _IsDisposed;
        private ActivityCompletion<IActivityResponse> _ActivityCompletion;
        private Guid _OperationId;
        private Guid _ExecutionId;
        private readonly AtomicReference<ImmutableList<IActivityEvent>> _ActivityEvents;

#if false
        public MediatorContext() {
            this._ActivityCompletion = new ActivityCompletion<IActivityResponse>();
            this._ActivityEvents = new AtomicReference<ImmutableList<IActivityEvent>>(
                    ImmutableList<IActivityEvent>.Empty
                );
        }
#endif

        public MediatorContext(CreateActivityContextArguments arguments, TRequest request) {
            this._ActivityCompletion = new ActivityCompletion<IActivityResponse>();
            this._ActivityEvents = new AtomicReference<ImmutableList<IActivityEvent>>(
                    ImmutableList<IActivityEvent>.Empty
                );
            this._MedaitorService = arguments.MedaitorService;
            this._Request = request;
            this.OperationId = Guid.NewGuid();
            this.ExecutionId = Guid.NewGuid();
        }

        public Type GetRequestType() => typeof(TRequest);

        public ActivityStatus Status { get; set; }

        public Guid OperationId {
            get {
                if (this._OperationId == Guid.Empty) {
                    lock (this) {
                        if (this._OperationId == Guid.Empty) {
                            this._OperationId = Guid.NewGuid();
                        }
                    }
                }
                return this._OperationId;
            }
            set {
                if (this.Status != ActivityStatus.Initialize && this._OperationId != Guid.Empty) {
                    throw new NotSupportedException($"{nameof(OperationId)} is already set.");
                }
                this._OperationId = value;
            }
        }

        public Guid ExecutionId {
            get {
                if (this._ExecutionId == Guid.Empty) {
                    lock (this) {
                        if (this._ExecutionId == Guid.Empty) {
                            this._ExecutionId = Guid.NewGuid();
                        }
                    }
                }
                return this._ExecutionId;
            }
            set {
                if (this.Status != ActivityStatus.Initialize && this._ExecutionId != Guid.Empty) {
                    throw new NotSupportedException($"{nameof(ExecutionId)} is already set.");
                }
                this._ExecutionId = value;
            }
        }

        public TRequest Request {
            get { return this._Request; }
            set { this._Request = value; }
        }

        public IActivityEvent[] ActivityEvents {
            get {
                return this._ActivityEvents.Value.ToArray();
            }
            set {
                this._ActivityEvents.Mutate((ignore) => ImmutableList<IActivityEvent>.Empty.AddRange(value));
            }
        }

        public IMediatorService MedaitorService {
            get { return this._MedaitorService; }
            set {
                if (ReferenceEquals(this._MedaitorService, value)) { return; }
                if (this._MedaitorService is object) {
                    throw new ArgumentException("already set", nameof(this.MedaitorService));
                }
                this._MedaitorService = value;
            }
        }

        public Task AddActivityEventAsync(IActivityEvent activityEvent) {
            if (activityEvent is null) { return Task.CompletedTask; }
            this._ActivityEvents.Mutate1<IActivityEvent>(
                activityEvent,
                (newItem, activityEvents) => {
                    newItem.SequenceNo = activityEvents.Count + 1;
                    return activityEvents.Add(newItem);
                });
            var result = this._MedaitorService.Storage.AddActivityEventAsync(activityEvent);

            return result;
        }

        public async Task SetActivityResponse(IActivityResponse activityResponse) {
            if (ReferenceEquals(this._ActivityResponse, activityResponse)) { return; }

            if (activityResponse is null) {
                throw new ArgumentNullException(nameof(activityResponse));
            }

            var prev = System.Threading.Interlocked.CompareExchange(ref this._ActivityResponse, activityResponse, null);
            if (prev is null) {
                var activityEvent = activityResponse.GetAsActivityEvent(this);
                var taskAddActivityEvent = this.AddActivityEventAsync(activityEvent);
                this._ActivityCompletion.TrySetResult(activityResponse);
                await taskAddActivityEvent;
            } else {
                throw new ArgumentException("already set", nameof(activityResponse));
            }
        }

        public Task SetFailure(Exception error) {
            return this.SetActivityResponse(new FailureActivityResponse(error));
        }

        public Task SetResponse(TResponse response) {
            return this.SetActivityResponse(new OkResultActivityResponse<TResponse>(response));
        }

        public Task<IActivityResponse> GetActivityResponseAsync() {
            return this._ActivityCompletion.Task;
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
                _ActivityCompletion.Dispose();
            }
        }
    }
}
