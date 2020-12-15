using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Utility;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Medaitor {
    public interface IMedaitorAccess {
        IUsingValue<IMedaitorClient> GetMedaitorClient();
    }
    public interface IMedaitorClient : IDisposable {
        IActivityContext<TRequest> CreateContextByRequest<TRequest>(
            TRequest request
            );
        IActivityContext<TRequest, TResponse> CreateContextByTypes<TRequest, TResponse>(
            TRequest request
            );


        Task SendAsync(
            IActivityContext medaitorContext,
            CancellationToken cancellationToken
            );

        Task WaitForAsync(
            IActivityContext medaitorContext,
            CancellationToken cancellationToken
            );
    }

    public interface IMedaitorService {

    }
}
