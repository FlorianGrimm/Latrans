using System;

namespace Brimborium.Latrans.ActivityContract {
    public interface IActivityRequest
    {
        string ExecutionId { get; set; }
        string Description { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime FinishedAt { get; set; }
        string JsonType { get; set; }
        string JsonData { get; set; }
    }
    public interface IActivityRequest<T>: IActivityRequest {
    }

    public interface IActivityResponce {
    }

    public interface IActivityResponce<T>: IActivityResponce {
    }

    public interface IActivityHandler<IActivityRequest, IActivityResponce>
    {
    }

    /*
     
    public class ActivityRequest:IActivityRequest
    {
    }
    public class ActivityResponce:IActivityResponce
    {
    }
      
    */
}
