using System;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Activity {

    public interface IActivityContext {
        ActivityStatus Status {get;set;}

        Guid OperationId { get; set; }

        Guid ExecutionId { get; set; }
        
        Type GetRequestType();

        Task AddActivityEvent(IActivityEvent activityEvent);

        Task SetFailure(System.Exception error);

        Task SetActivityResponse(IActivityResponse activityResponse);

        Task<IActivityResponse> GetActivityResponseAsync();
    }

    public interface IActivityContext<TRequest> : IActivityContext {
        TRequest Request { get; set; }
    }

    public interface IActivityContext<TRequest, TResponse>
        : IActivityContext<TRequest>
        , IDisposable {
        Task SetResponse(TResponse response);
    }
}
