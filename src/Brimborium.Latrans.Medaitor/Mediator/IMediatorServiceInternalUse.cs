
using Brimborium.Latrans.Activity;

using System;

namespace Brimborium.Latrans.Mediator {
    /// <summary>
    /// public accessor - but internal use.
    /// </summary>
    public interface IMediatorServiceInternalUse : IMediatorService {
#warning TODO CreateContext<TRequest, TResponse>
        IActivityContext<TRequest> CreateContext<TRequest, TResponse>(
                ActivityId activityId,
                TRequest request,
                RequestRelatedType? requestRelatedType
            );


        void HandleRequestForAccepted202Redirect<TRequest>(
                IActivityContext<TRequest> activityContext
            );

        void HandleRequestAfterTimeout<TRequest>(
                IActivityContext<TRequest> activityContext
            );

    }

    public interface IMediatorScopeServiceInternalUse : IMediatorScopeService {
#warning TODO CreateHandler<TRequest, TResponse>
        IActivityHandler<TRequest, TResponse> CreateHandler<TRequest, TResponse>(
                RequestRelatedType? requestRelatedType,
                IActivityContext<TRequest> activityContext
            )
        where TRequest : IRequest<TResponse>, IRequestBase
        where TResponse : IResponseBase
            ;
    }
}
