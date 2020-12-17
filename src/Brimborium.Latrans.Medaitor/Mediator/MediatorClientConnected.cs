using Brimborium.Latrans.Activity;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {
    public class MediatorClientConnected<TRequest, TResponse>
        : IMediatorClientConnected<TRequest, TResponse>
        , IMediatorClientConnectedInternal<TRequest>
        , IMediatorClientConnected<TRequest>
        , IMediatorClientConnected
        , IDisposable {
        private IActivityContext<TRequest, TResponse> _ActivityContext;
        private readonly IMediatorServiceInternal _MedaitorService;
        private readonly RequestRelatedType _RequestRelatedType;
        private readonly TRequest _Request;
        private int _IsDisposed;

        public MediatorClientConnected() {
        }

        public MediatorClientConnected(CreateClientConnectedArguments arguments, TRequest request) {
            var medaitorService = arguments.MedaitorService;
            this._MedaitorService = medaitorService;
            this._RequestRelatedType = arguments.RequestRelatedType;
            this._Request = request;
        }

        public async Task<IMediatorClientConnected<TRequest>> SendAsync(
                CancellationToken cancellationToken
            ) {
            //
            var medaitorService = this._MedaitorService;

            var activityContext = this._ActivityContext ??= medaitorService.CreateContext<TRequest, TResponse>(this._RequestRelatedType, this._Request);
            //
            var activityHandler = (IActivityHandler<TRequest, TResponse>)medaitorService.CreateHandler<TRequest, TResponse>(
                this._RequestRelatedType,
                activityContext);
            await activityContext.AddActivityEventAsync(new ActivityEventStateChange(
                activityContext.OperationId,
                activityContext.ExecutionId,
                0,
                System.DateTime.UtcNow,
                ActivityStatus.Running
                ));
            await activityHandler.ExecuteAsync(activityContext, cancellationToken);

            return this;
        }

        public async Task<IActivityResponse> WaitForAsync(
            ActivityWaitForSpecification waitForSpecification,
            CancellationToken cancellationToken
            ) {
            waitForSpecification ??= ActivityWaitForSpecification.Default;
            if (waitForSpecification.RespectRequestAborted) {
                if (cancellationToken.IsCancellationRequested) {
                    if (waitForSpecification.SupportAccepted202Redirect) {
                        var redirectUrl = waitForSpecification.RedirectUrl(this._ActivityContext.ExecutionId);
                        return new AcceptedActivityResponse(redirectUrl);
                    } else {
                        return new CanceledActivityResponse();
                    }
                }
            }
            if (waitForSpecification.WaitTimeSpan == TimeSpan.MaxValue) {
                var result = await this._ActivityContext.GetActivityResponseAsync();
                return result;
            } else {
                var taskResult = this._ActivityContext.GetActivityResponseAsync();
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
                        this._MedaitorService.HandleRequestForAccepted202Redirect(this._ActivityContext);
                        var redirectUrl = waitForSpecification.RedirectUrl(this._ActivityContext.ExecutionId);
                        return new AcceptedActivityResponse(redirectUrl);
                    } else {
                        this._MedaitorService.HandleRequestAfterTimeout(this._ActivityContext);
                        return new CanceledActivityResponse();
                    }
                }
            }
            throw new NotImplementedException();
        }

        ~MediatorClientConnected() {
            this.Dispose(disposing: false);
        }

        public void Dispose() {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (0 == System.Threading.Interlocked.Exchange(ref this._IsDisposed, 1)) {
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
