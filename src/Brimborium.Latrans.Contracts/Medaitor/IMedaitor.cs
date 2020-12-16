using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Utility;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Medaitor {
    public interface IMedaitorAccess {
        IMedaitorClient GetMedaitorClient();
    }
    public interface IMedaitorClient : IDisposable {
        bool IsDisposed {get;}

        Task<IMedaitorClientConnected<TRequest>> ConnectAsync<TRequest>(
            TRequest request
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

    public interface IMedaitorClientConnected : IDisposable {
    }
    public interface IMedaitorClientConnected<TRequest> : IMedaitorClientConnected {
        Task<IActivityResponse> WaitForAsync(
           IActivityContext activityContext,
           ActivityWaitForSpecification waitForSpecification,
           CancellationToken cancellationToken
           );
    }
    public interface IMedaitorClientConnectedInternal<TRequest> : IMedaitorClientConnected<TRequest> {
        Task<IMedaitorClientConnected<TRequest>> SendAsync();
    }

    public interface IMedaitorClientConnected<TRequest, TResponse> : IMedaitorClientConnected<TRequest> {
    }

        public interface IMedaitorService : IDisposable {
        IActivityContext<TRequest> CreateContextByRequest<TRequest>(
            IMedaitorClient medaitorClient,
            TRequest request
            );

        Task SendAsync(
            IMedaitorClient medaitorClient,
            IActivityContext activityContext, 
            CancellationToken cancellationToken
            );

        Task<IActivityResponse> WaitForAsync(
            IMedaitorClient medaitorClient,
            IActivityContext activityContext,
            ActivityWaitForSpecification waitForSpecification,
            CancellationToken cancellationToken
            );


        Task<IMedaitorClientConnected<TRequest>> ConnectAsync<TRequest>(
            IMedaitorClient medaitorClient,
            TRequest request);
    }
}
