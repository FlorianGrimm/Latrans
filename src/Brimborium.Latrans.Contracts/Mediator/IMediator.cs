using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Utility;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {
    public interface IMediatorAccess {
        IMediatorClient GetMedaitorClient();
    }
    // 
    public interface IMediatorClient : IDisposable {
        bool IsDisposed { get; }

        Task<IMediatorClientConnected<TRequest>> ConnectAsync<TRequest>(
            TRequest request,
            CancellationToken cancellationToken
            );

        IActivityContext<TRequest> CreateContextByRequest<TRequest>(
            TRequest request
            );

        Task SendAsync(
            IActivityContext activityContext,
            CancellationToken cancellationToken
            );

        Task WaitForAsync(
            IActivityContext activityContext,
            ActivityWaitForSpecification waitForSpecification,
            CancellationToken cancellationToken
            );
    }

    public interface IMediatorClientConnected : IDisposable {
    }

    public interface IMediatorClientConnected<TRequest> : IMediatorClientConnected {
        Task<IActivityResponse> WaitForAsync(
           ActivityWaitForSpecification waitForSpecification,
           CancellationToken cancellationToken
           );
    }

    public interface IMediatorClientConnectedInternal<TRequest> : IMediatorClientConnected<TRequest> {
        Task<IMediatorClientConnected<TRequest>> SendAsync(
                CancellationToken cancellationToken
            );
    }

    public interface IMediatorClientConnected<TRequest, TResponse> : IMediatorClientConnected<TRequest> {
    }

    public interface IMediatorService : IDisposable {
        IActivityContext<TRequest> CreateContextByRequest<TRequest>(
            IMediatorClient medaitorClient,
            TRequest request
            );

        Task SendAsync(
            IMediatorClient medaitorClient,
            IActivityContext activityContext,
            CancellationToken cancellationToken
            );

        Task<IActivityResponse> WaitForAsync(
            IMediatorClient medaitorClient,
            IActivityContext activityContext,
            ActivityWaitForSpecification waitForSpecification,
            CancellationToken cancellationToken
            );

        ///////////////////////
        
        Task<IMediatorClientConnected<TRequest>> ConnectAsync<TRequest>(
            IMediatorClient medaitorClient,
            TRequest request,
            CancellationToken cancellationToken);
        
        //IActivityContext<TRequest, TResponse> CreateContext<TRequest, TResponse>(
        //        IRequestRelatedType requestRelatedType,
        //        TRequest request
        //    );

        IActivityHandler<TRequest, TResponse> CreateHandler<TRequest, TResponse>(
                IRequestRelatedType requestRelatedType,
                IActivityContext<TRequest, TResponse> activityContext
            );
    }

    public interface IRequestRelatedType {
        public Type DispatcherType { get; set; }
        public Type[] HandlerTypes { get; set; }
    }
}
