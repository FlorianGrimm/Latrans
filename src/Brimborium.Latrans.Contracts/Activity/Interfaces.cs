using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Activity {
    public interface IActivityExecution {
        /*
        ActivityStatus Status { get; set; }
        string ExecutionId { get; set; }
        string Description { get; set; }
        DateTime StartAt { get; set; }
        DateTime FinishedAt { get; set; }
        string JsonType { get; set; }
        string JsonData { get; set; }
        */
    }
    public interface IActivityExecutionChange {
    }


    public interface IActivityHandler {
        /*
        Task SendAsync(
            IActivityContext activityContext,
            CancellationToken cancellationToken
            );
         */
    }

    public interface IActivityHandler<TRequest> : IActivityHandler
        where TRequest : IRequestBase {
        Task ExecuteAsync(
            IActivityContext<TRequest> activityContext,
            CancellationToken cancellationToken
            );
    }

    public interface IActivityHandler<TRequest, TResponse> 
        : IActivityHandler<TRequest>
        where TRequest : IRequest<TResponse>, IRequestBase
        where TResponse : IResponseBase {
    }

    public interface IDispatchActivityHandler {
    }

    public interface IDispatchActivityHandler<TRequest>
        : IDispatchActivityHandler {
    }

    public interface IDispatchActivityHandler<TRequest, TResponse>
        : IDispatchActivityHandler<TRequest>
        where TRequest : IRequest<TResponse>, IRequestBase
        where TResponse : IResponseBase {
        IActivityHandler<TRequest, TResponse> GetActivityHandler(
            Type[] handlerTypes,
            IActivityContext<TRequest> activityContext,
            Func<Type, IActivityHandler<TRequest, TResponse>> createActivityHandlerType                
            )
        
            ;
    }
}
