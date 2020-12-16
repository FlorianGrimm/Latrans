using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Medaitor;
using Brimborium.Latrans.Utility;

using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Medaitor {
    public class MedaitorContext<TRequest, TResponse>
        : IActivityContext<TRequest, TResponse>
        , IDisposable {

        public static Func<IServiceProvider, object, IActivityContext> GetCreateInstance()
            => ((IServiceProvider serviceProvider, object request) => new MedaitorContext<TRequest, TResponse>(serviceProvider, (TRequest)request));

        private TRequest _Request;
        private IActivityResponse _ActivityResponse;
        private int _IsDisposed;
        private ActivityCompletion<IActivityResponse> _ActivityCompletion;
        private Guid _ExecutionId;
        private IServiceProvider _ServiceProvider;

        public MedaitorContext() {
            this._ActivityCompletion = new ActivityCompletion<IActivityResponse>();
        }

        public MedaitorContext(IServiceProvider serviceProvider, TRequest request) {
            this._ActivityCompletion = new ActivityCompletion<IActivityResponse>();
            this.ExecutionId = Guid.NewGuid();
            this._ServiceProvider = serviceProvider;
            this._Request = request;
        }

        public Type GetRequestType() => typeof(TRequest);

        public Guid ExecutionId {
            get { return this._ExecutionId; }
            set {
                if (this._ExecutionId != Guid.Empty) {
                    throw new NotSupportedException($"{nameof(ExecutionId)} is already set.");
                }
                this._ExecutionId = value;
            }
        }

        public TRequest Request {
            get { return this._Request; }
            set { this._Request = value; }
        }

        public IServiceProvider ServiceProvider {
            get { return this._ServiceProvider; }
            set {
                if (ReferenceEquals(this._ServiceProvider, value)) { return; }
                if (this._ServiceProvider is object) {
                    throw new ArgumentException("already set", nameof(this.ServiceProvider));
                }
                this._ServiceProvider = value;
            }
        }

        public void AddActivityEvent(IActivityEvent activityEvent) {
            throw new NotImplementedException();
        }

        public void SetActivityResponse(IActivityResponse activityResponse) {
            if (activityResponse is object) {
                if (activityResponse is FailureActivityResponse) {
                    this.SetActivityResponseInternal(activityResponse);
                    return;
                }

                //        try {
                //            throw new NotSupportedException();
                //        } catch (System.Exception error) {
                //            this._ActivityResponse = new FailureActivityResponse(error);
                //        }

                if (this._ActivityResponse is object) {
                    if (this._ActivityResponse is FailureActivityResponse) {
                        this.SetActivityResponseInternal(activityResponse);
                        return;
                    }
                } else {
                    this.SetActivityResponseInternal(activityResponse);
                    return;
                }
            }
        }

        void SetActivityResponseInternal(IActivityResponse activityResponse) {
            this._ActivityResponse = activityResponse;
            this._ActivityCompletion.TrySetResult(activityResponse);
            //if (activityResponse is IFailureActivityResponse failureActivityResponse) {
            //    var error = (failureActivityResponse.GetError()) ?? (new Exception("Failed"));
            //    this._ActivityCompletion.TrySetException(error);
            //} else {
            //}
        }

        public void SetFailure(Exception error) {
            this.SetActivityResponse(new FailureActivityResponse(error));
        }

        public void SetResponse(TResponse response) {
            this.SetActivityResponse(new OkResultActivityResponse<TResponse>(response));
        }

        public Task<IActivityResponse> GetActivityResponseAsync() {
            return this._ActivityCompletion.Task;
        }

        ~MedaitorContext() {
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
