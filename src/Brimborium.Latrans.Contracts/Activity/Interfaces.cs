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

    public interface IRequestBase {
    }

    public interface IResponseBase {
    }

    public interface IRequest<out TResponse> : IRequestBase
       where TResponse : IResponseBase {
    }

    public interface IActivityResult {
    }

    public interface IActivityContext {
    }
    public interface IActivityContext<TRequset> : IActivityContext {
        TRequset Request { get; }
        //void SetResponse(TResponse response);
        //void SetFailure(System.Exception error);
        //void SetActivityResult(IActivityResult medaitorResult);
    }
    public interface IActivityContext<TRequset, TResponse> : IActivityContext {
        TRequset Request { get; }
        void SetResponse(TResponse response);
        void SetFailure(System.Exception error);
        void SetActivityResult(IActivityResult medaitorResult);
    }

    public interface IActivityHandler<TRequset, TResponse> {
        Task ExecuteAsync(
            IActivityContext<TRequset, TResponse> medaitorContext,
            CancellationToken cancellationToken
            );
    }
}
