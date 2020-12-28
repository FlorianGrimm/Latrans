using Brimborium.Latrans.Activity;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {
    public class MediatorClientConnected<TRequest, TResponse>
        : IMediatorClientConnected<TRequest, TResponse>
        , IMediatorClientConnected<TRequest>
        , IMediatorClientConnected
        , IDisposable
        where TRequest : IRequest<TResponse>, IRequestBase
        where TResponse : IResponseBase {
        private IActivityContext<TRequest, TResponse>? _ActivityContext;
        private readonly IMediatorServiceInternalUse _MedaitorService;
        private readonly IMediatorClient? _MedaitorClient;
        private readonly IMediatorScopeServiceInternalUse? _MediatorScopeService;
        private readonly RequestRelatedType _RequestRelatedType;
        private readonly TRequest _Request;
        private readonly ActivityId _ActivityId;
        private int _IsDisposed;
        private bool _ClientConnected;

        //public MediatorClientConnected(
        //        IMediatorServiceInternalUse2 medaitorService,
        //        RequestRelatedType requestRelatedType,
        //        TRequest request
        //    ) {
        //    this._MedaitorService = medaitorService;
        //    this._RequestRelatedType = requestRelatedType;
        //    this._Request = request;
        //}

        [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor()]
        public MediatorClientConnected(CreateClientConnectedArguments arguments, TRequest request) {
            this._MedaitorService = arguments.MedaitorService;
            this._MedaitorClient= arguments.MedaitorClient;
            this._MediatorScopeService = arguments.MediatorScopeService;
            this._ActivityId = arguments.ActivityId;
            this._Request = request;
            this._RequestRelatedType = arguments.RequestRelatedType;
        }

        public void Initialize() {
            var medaitorService = this._MedaitorService;

            var activityContext = this._ActivityContext ??= medaitorService.CreateContext<TRequest, TResponse>(
                this._ActivityId,
                this._Request,
                this._RequestRelatedType
                );

            var mediatorScopeService = (IMediatorScopeServiceInternalUse)activityContext.MediatorScopeService;
            mediatorScopeService.AddClientConnected<TRequest>(this);
            this._ClientConnected = true;
        }

        public IActivityContext? GetActivityContext()
            => this._ActivityContext;

        public async Task SendAsync(
                CancellationToken cancellationToken
            ) {
            //
            var activityContext = this._ActivityContext;
            if (this._ActivityContext is null) {
                this.Initialize();
                activityContext = this._ActivityContext;
            }
            if (activityContext is null) {
                throw new InvalidOperationException("ActivityContext is null");
            }
            var medaitorService = this._MedaitorService;
            if (medaitorService is null) {
                throw new InvalidOperationException("MedaitorService is null");
            }
            
            var mediatorScopeService = (IMediatorScopeServiceInternalUse)activityContext.MediatorScopeService;

            var activityHandler = mediatorScopeService.CreateHandler<TRequest, TResponse > (
                  this._RequestRelatedType,
                  activityContext);
            await activityContext.AddActivityEventAsync(new ActivityEventStateChange(
                activityContext.ActivityId,
                0,
                System.DateTime.UtcNow,
                ActivityStatus.Running
                ));
            
            await activityHandler.ExecuteAsync(activityContext, cancellationToken);

            return;
        }

        public async Task<IActivityResponse> WaitForAsync(
            ActivityExecutionConfiguration waitForSpecification,
            CancellationToken cancellationToken
            ) {
            waitForSpecification ??= ActivityExecutionConfiguration.Default;
            var activityContext = this._ActivityContext;
            if (activityContext is null) {
                throw new InvalidOperationException("ActivityContext is null");
            }
            //
            var taskResult = activityContext.GetActivityResponseAsync();
            if (taskResult.IsCompletedSuccessfully) {
                return await taskResult;
            }
            //
            if (waitForSpecification.RespectRequestAborted) {
                if (cancellationToken.IsCancellationRequested) {
                    if (waitForSpecification.SupportAccepted202Redirect) {
                        var redirectUrl = waitForSpecification.RedirectUrl(activityContext.ActivityId.ExecutionId);
                        return new AcceptedActivityResponse(redirectUrl);
                    } else {
                        return new CanceledActivityResponse();
                    }
                }
            }
            if (waitForSpecification.WaitTimeSpan == TimeSpan.MaxValue) {
                var result = await activityContext.GetActivityResponseAsync();
                return result;
            } else {
                var cts = (waitForSpecification.RespectRequestAborted)
                    ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken)
                    : new CancellationTokenSource();
                var taskTimeout = Task.Delay(waitForSpecification.WaitTimeSpan, cts.Token);
                var taskDone = await Task.WhenAny(taskResult, taskTimeout);
                if (ReferenceEquals(taskResult, taskDone)) {
                    // success
                    cts.Cancel();
                    try { await taskTimeout; } catch (TaskCanceledException) { }
                    return await taskResult;
                } else {
                    // timeout
                    if (waitForSpecification.SupportAccepted202Redirect) {
                        this._MedaitorService.HandleRequestForAccepted202Redirect(activityContext);
                        var redirectUrl = waitForSpecification.RedirectUrl(activityContext.ActivityId.ExecutionId);
                        return new AcceptedActivityResponse(redirectUrl);
                    } else {
                        this._MedaitorService.HandleRequestAfterTimeout(activityContext);
                        return new CanceledActivityResponse();
                    }
                }
            }
        }

        public async Task<MediatorActivityStatus> GetStatusAsync() {
            var activityContext = this._ActivityContext;            
            if (activityContext is object) {
                return await activityContext.GetStatusAsync();
            } else {
                return new MediatorActivityStatus() {
                    Status = ActivityStatus.Unknown
                };
            }
        }

        public bool IsDisposed()
            => (this._IsDisposed != 0);

        ~MediatorClientConnected() {
            this.Dispose(disposing: false);
        }

        public void Dispose() {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (0 == System.Threading.Interlocked.Exchange(ref this._IsDisposed, 1)) {
                if (this._ClientConnected) {
                    this._ClientConnected = false;
                    var activityContext = this._ActivityContext;
                    if (activityContext is null) {
                        //
                    } else { 
                        var mediatorScopeService = (IMediatorScopeServiceInternalUse)activityContext.MediatorScopeService;
                        mediatorScopeService.RemoveClientConnected<TRequest>(this);
                    }
                }
                //if (disposing) {
                //    if (_ActivityCompletion.IsNotDefined) {
                //        _ActivityCompletion.TrySetResult(new FailureActivityResponse(new Exception("Disposing")));
                //    }
                //} else {
                //    if (_ActivityCompletion.IsNotDefined) {
                //        _ActivityCompletion.TrySetResult(new FailureActivityResponse(new Exception("Disposed")));
                //    }
                //}
                //_ActivityCompletion.Dispose();
            }
        }
    }
}
