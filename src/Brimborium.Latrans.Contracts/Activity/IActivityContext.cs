﻿using Brimborium.Latrans.Mediator;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Activity {

    public interface IActivityContext 
        : IMediatorClient {

        ActivityStatus Status {get;set;}

        ActivityId ActivityId { get; set; }
        
        //Guid OperationId { get; set; }
        //Guid ExecutionId { get; set; }
        
        Type GetRequestType();

        Task AddActivityEventAsync(IActivityEvent activityEvent);

        Task SetFailureAsync(System.Exception error);

        Task SetActivityResponseAsync(IActivityResponse activityResponse);

        Task<IActivityResponse> GetActivityResponseAsync();

        IMediatorScopeService MediatorScopeService { get; }

        Task<MediatorActivityStatus> GetStatusAsync();
    }

    public interface IActivityContext<TRequest> : IActivityContext {
        TRequest Request { get; set; }
    }

    public interface IActivityContext<TRequest, TResponse>
        : IActivityContext<TRequest>
        , IDisposable {
        Task SetResponseAsync(TResponse response);
    }
}
