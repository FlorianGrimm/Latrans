using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Medaitor;

using System;

namespace Brimborium.Latrans.Medaitor{
    public class MedaitorContext<TRequset, TResponse>
        : IActivityContext<TRequset, TResponse> {
        public TRequset Request => throw new NotImplementedException();

        public void SetActivityResult(IActivityResult medaitorResult) {
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
