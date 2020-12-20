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
        Task SendAsync(
            IActivityContext activityContext,
            CancellationToken cancellationToken
            );
    }

    public interface IActivityHandler<TRequest> : IActivityHandler {
    }

    public interface IActivityHandler<TRequest, TResponse> : IActivityHandler<TRequest> {
        Task ExecuteAsync(
            IActivityContext<TRequest, TResponse> activityContext,
            CancellationToken cancellationToken
            );
    }

    public interface IDispatchActivityHandler {
    }

    public interface IDispatchActivityHandler<TRequest>
        : IDispatchActivityHandler {
    }

    public interface IDispatchActivityHandler<TRequest, TResponse>
        : IDispatchActivityHandler<TRequest> {
        IActivityHandler<TRequest, TResponse> GetActivityHandler(
            Type[] handlerTypes,
            IActivityContext<TRequest, TResponse> activityContext,
            Func<Type, IActivityHandler<TRequest, TResponse>> createActivityHandlerType
            );
    }
}
