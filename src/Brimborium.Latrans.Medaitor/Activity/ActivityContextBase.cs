using System;
using System.Collections.Generic;
using System.Text;

namespace Brimborium.Latrans.Activity {
    public class ActivityContextBase : IActivityContext {
        protected ActivityContextBase() {
        }
    }
    public class ActivityContext<TRequest, TResponse>
        : ActivityContextBase
        , IActivityContext<TRequest, TResponse> {
        private readonly TRequest _Request;

        public ActivityContext(TRequest request) {            
            this._Request= request;
        }

        public TRequest Request => this._Request;

        public void SetResponse(TResponse response) {
            throw new NotImplementedException();
        }

        public void SetFailure(Exception error) {
            throw new NotImplementedException();
        }

        public void SetActivityResult(IActivityResult medaitorResult) {
            throw new NotImplementedException();
        }
    }
}
