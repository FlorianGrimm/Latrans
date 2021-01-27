using Brimborium.Latrans.Mediator;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Activity {

    public interface IActivityContext
        : IMediatorClient {

        ActivityStatus Status { get; set; }

        ActivityId ActivityId { get; set; }

        Type GetRequestType();

        /// <summary>
        /// Internal
        /// </summary>
        /// <param name="activityEvent"></param>
        /// <returns></returns>
        Task AddActivityEventAsync(IActivityEvent activityEvent);

        Task SetFailureAsync(System.Exception error);

        Task SetActivityResponseAsync(IActivityResponse activityResponse);

        Task<IActivityResponse> GetActivityResponseAsync();

        IMediatorScopeService MediatorScopeService { get; }

        Task<MediatorActivityStatus> GetStatusAsync();
    }

    public interface IActivityContext<TRequest>
        : IActivityContext {
        TRequest Request { get; set; }
    }
}
