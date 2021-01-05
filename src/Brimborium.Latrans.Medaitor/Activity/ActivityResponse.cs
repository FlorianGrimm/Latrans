using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Brimborium.Latrans.Activity {
    [DataContract]
    public class OkResultActivityResponse<T>
        : IActivityResponse
        , IOkResultActivityResponse {
        [DataMember]
        public T Result { get; set; }

        public OkResultActivityResponse(T result) {
            this.Result = result;
        }

        public IActivityEvent GetAsActivityEvent(IActivityContext activityContext) {
            return new ActivityEventStateChange(
                activityContext.ActivityId,
                -1,
                System.DateTime.UtcNow,
                ActivityStatus.Completed,
                null);
        }

        public bool TryGetResult([MaybeNullWhen(false)] out object value) {
            if (this.Result is object) {
                value = this.Result;
                return true;
            } else {
                value = default;
                return false;
            }
        }
    }

    [DataContract]
    public class CanceledActivityResponse : IActivityResponse {
        public CanceledActivityResponse() {
        }

        public IActivityEvent GetAsActivityEvent(IActivityContext activityContext) {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    public class AcceptedActivityResponse : IActivityResponse {
        public AcceptedActivityResponse() {
        }
        public AcceptedActivityResponse(string redirectUrl) {
            this.RedirectUrl = redirectUrl;
        }

        [DataMember]
        public string? RedirectUrl { get; set; }

        public IActivityEvent GetAsActivityEvent(IActivityContext activityContext) {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    public class FailureActivityResponse : IFailureActivityResponse {
        public FailureActivityResponse() {
        }

        public FailureActivityResponse(Exception error) {
            this.Error = error;
        }

        [DataMember]
        public Exception? Error { get; set; }

        public IActivityEvent GetAsActivityEvent(IActivityContext activityContext) {
            throw new NotImplementedException();
        }

        public Exception? GetError() => this.Error;
    }
}
