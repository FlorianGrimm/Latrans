using System;

namespace Brimborium.Latrans.Activity {
    public class ActivityContextFactory {
        //<TRequset, TResponse>
        public IActivityContext<TRequset, TResponse> CreateContext<TRequset, TResponse>(
            TRequset requset
            ) {
            throw new NotImplementedException();
        }

    }
}
