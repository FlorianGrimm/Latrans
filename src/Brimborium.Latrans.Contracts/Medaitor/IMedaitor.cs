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

    public interface IMedaitorService : IDisposable {
        IActivityContext<TRequest> CreateContextByRequest<TRequest>(
            IMedaitorClient medaitorClient,
            TRequest request
            );
        Task SendAsync(
            IMedaitorClient medaitorClient,
            IActivityContext activityContext, 
            CancellationToken cancellationToken);
        Task WaitForAsync(
            IMedaitorClient medaitorClient,
            IActivityContext activityContext,
            ActivityWaitForSpecification waitForSpecification,
            CancellationToken cancellationToken
            );
    }
}
