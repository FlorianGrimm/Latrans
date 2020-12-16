using System;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Activity {
    
    public interface IActivityContext {
        Type GetRequestType();
        
        void AddActivityEvent(IActivityEvent activityEvent);

        void SetFailure(System.Exception error);

        void SetActivityResponse(IActivityResponse activityResponse);

        Task<IActivityResponse> GetActivityResponseAsync();
    }

    public interface IActivityContext<TRequest> : IActivityContext {
        TRequest Request { get; set; }
    }

    public interface IActivityContext<TRequest, TResponse>
        : IActivityContext<TRequest>
        , IDisposable {
        void SetResponse(TResponse response);
    }
}
