using Brimborium.Latrans.Activity;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {

    public interface IMediatorClient : IDisposable {
        bool IsDisposed { get; }

        Task<IMediatorClientConnected<TRequest>> ConnectAndSendAsync<TRequest>(
                ActivityId activityId,
                TRequest request,
                ActivityExecutionConfiguration activityExecutionConfiguration,
                CancellationToken cancellationToken
            );

        Task<IMediatorClientConnected<TRequest>> ConnectAsync<TRequest>(
                ActivityId activityId,
                CancellationToken cancellationToken
            );

        Task<MediatorActivityStatus> GetStatusAsync();

    }
}
