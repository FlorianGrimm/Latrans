using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Medaitor;

using System;

namespace Brimborium.Latrans.Medaitor {
    public class MedaitorContext<TRequest, TResponse>
        : IActivityContext<TRequest, TResponse>
        , IActivityContextInternal<TRequest> {
        private TRequest _Request;

        public MedaitorContext() {
        }
        public Type GetRequestType() => typeof(TRequest);

        public TRequest Request {
            get { return this._Request; }
            set { this._Request = value; }
        }

        public void SetRequest(TRequest request) {
            this._Request = request;
        }


        public void SetActivityResponse(IActivityResponse medaitorResult) {
            throw new NotImplementedException();
        }

        public void SetFailure(Exception error) {
            throw new NotImplementedException();
        }

        public void SetResponse(TResponse response) {
            throw new NotImplementedException();
        }
    }
}
