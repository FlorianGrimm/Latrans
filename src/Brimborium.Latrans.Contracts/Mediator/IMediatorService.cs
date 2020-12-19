using Brimborium.Latrans.Utility;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {
    public interface IMediatorService : IDisposable {
        IMediatorServiceStorage Storage { get; }

        Task<IMediatorClientConnected<TRequest>> ConnectAsync<TRequest>(
            IMediatorClient medaitorClient,
            TRequest request,
            CancellationToken cancellationToken);
    }
}
