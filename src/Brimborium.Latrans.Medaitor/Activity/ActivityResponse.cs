using System;
using System.Collections.Generic;
using System.Text;

namespace Brimborium.Latrans.Activity {
    public class OkResultActivityResponse<T> : IActivityResponse {
        public T Result { get; set; }

        public OkResultActivityResponse(T result) {
            this.Result = result;
        }
    }

    public class AcceptActivityResponse : IActivityResponse {
        public AcceptActivityResponse() {
        }
    }

    public class FailureActivityResponse : IFailureActivityResponse {
        public FailureActivityResponse() {
        }

        public FailureActivityResponse(Exception error) {
            this.Error = error;
        }

        public Exception Error { get; set; }

        Exception IFailureActivityResponse.GetError() => this.Error;
    }
}
