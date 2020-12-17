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

        /// <summary>
        /// Internal use.
        /// Used in <see cref="MediatorBuilder.AddHandler{THandler}"/>.
        /// </summary>
        /// <returns>Function that creates the context.</returns>
        public static Func<CreateClientConnectedArguments, object, IMediatorClientConnected> GetCreateInstance()
            => ((CreateClientConnectedArguments arguments, object request) => new MediatorClientConnected<TRequest, TResponse>(arguments, (TRequest)request));

        private readonly IActivityContext<TRequest, TResponse> _ActivityContext;
        private readonly IMediatorServiceInternal _MedaitorService;
        private int _IsDisposed;
        private IActivityHandler<TRequest, TResponse> _ActivityHandler;

        public MediatorClientConnected() {
        }

        public MediatorClientConnected(CreateClientConnectedArguments arguments, TRequest request) {
            var medaitorService = arguments.MedaitorService;
            this._MedaitorService = medaitorService;
            //
            // var activityContext = (IActivityContext<TRequest, TResponse>)medaitorService.CreateContext<TRequest, TResponse>(arguments.RequestRelatedType, request);
            var activityContext = (IActivityContext<TRequest, TResponse>)arguments.RequestRelatedType.CreateActivityContext(
                new CreateActivityContextArguments() {
                    MedaitorService = medaitorService
                },
                request);
            this._ActivityContext = activityContext;
            //
            var activityHandler = (IActivityHandler<TRequest, TResponse>)medaitorService.CreateHandler<TRequest, TResponse>(
                arguments.RequestRelatedType,
                activityContext);
            this._ActivityHandler = activityHandler;
        }

        public async Task<IMediatorClientConnected<TRequest>> SendAsync(
                CancellationToken cancellationToken
            ) {
#warning TODO this._ActivityContext.AddActivityEvent();
            await this._ActivityHandler.ExecuteAsync(this._ActivityContext, cancellationToken);
#warning TODO check this._ActivityContext.Status
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
                        this._MedaitorService.AddRequestForAccepted202Redirect(this._ActivityContext);
                        var redirectUrl = waitForSpecification.RedirectUrl(this._ActivityContext.ExecutionId);
                        return new AcceptedActivityResponse(redirectUrl);
                    } else {
                        this._MedaitorService.AddRequestAfterTimeout(this._ActivityContext);
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
