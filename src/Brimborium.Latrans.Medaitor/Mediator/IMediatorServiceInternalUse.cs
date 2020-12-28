
using Brimborium.Latrans.Activity;

using System;

namespace Brimborium.Latrans.Mediator {
    /// <summary>
    /// public accessor - but internal use.
    /// </summary>
    public interface IMediatorServiceInternalUse : IMediatorService {
        IActivityContext<TRequest, TResponse> CreateContext<TRequest, TResponse>(
                ActivityId activityId,
                TRequest request,
                RequestRelatedType? requestRelatedType
            );


        void HandleRequestForAccepted202Redirect<TRequest, TResponse>(
                IActivityContext<TRequest, TResponse> activityContext
            );

        void HandleRequestAfterTimeout<TRequest, TResponse>(
                IActivityContext<TRequest, TResponse> activityContext
            );

    }

    public interface IMediatorScopeServiceInternalUse : IMediatorScopeService {
        
        IActivityHandler<TRequest, TResponse> CreateHandler<TRequest, TResponse>(
                RequestRelatedType? requestRelatedType,
                IActivityContext<TRequest, TResponse> activityContext
            )
        where TRequest : IRequest<TResponse>, IRequestBase
        where TResponse : IResponseBase
            ;
        
    }
}
