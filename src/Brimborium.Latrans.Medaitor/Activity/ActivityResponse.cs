using System;
using System.Collections.Generic;
using System.Text;

namespace Brimborium.Latrans.Activity {
    public class OkResultActivityResponse<T>
        : IActivityResponse
        , IOkResultActivityResponse
        //, IActivityEventChangeStatus
        {
        
        public T Result { get; set; }


        public OkResultActivityResponse(T result) {
            this.Result = result;
        }

        public IActivityEvent GetAsActivityEvent(IActivityContext activityContext) {
            return new ActivityEventStateChange(
                activityContext.ActivityId,
                -1,
                System.DateTime.UtcNow,
                ActivityStatus.Completed);
        }
    }

    public class CanceledActivityResponse : IActivityResponse {
        public CanceledActivityResponse() {
        }

        public IActivityEvent GetAsActivityEvent(IActivityContext activityContext) {
            throw new NotImplementedException();
        }
    }

    public class AcceptedActivityResponse : IActivityResponse {
        public AcceptedActivityResponse() {
        }
        public AcceptedActivityResponse(string redirectUrl) {
            this.RedirectUrl = redirectUrl;
        }

        public string? RedirectUrl { get; set; }

        public IActivityEvent GetAsActivityEvent(IActivityContext activityContext) {
            throw new NotImplementedException();
        }
    }

    public class FailureActivityResponse : IFailureActivityResponse {
        public FailureActivityResponse() {
        }

        public FailureActivityResponse(Exception error) {
            this.Error = error;
        }

        public Exception? Error { get; set; }

        public IActivityEvent GetAsActivityEvent(IActivityContext activityContext) {
            throw new NotImplementedException();
        }

        Exception? IFailureActivityResponse.GetError() => this.Error;
    }
}
