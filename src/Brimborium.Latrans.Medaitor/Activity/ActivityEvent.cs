using System;
using System.Runtime.Serialization;

namespace Brimborium.Latrans.Activity {
    [DataContract]
    public class ActivityEvent : IActivityEvent {
        public ActivityEvent() {
        }
        public ActivityEvent(
            ActivityId activityId,
            int sequenceNo,
            DateTime occurrence
        ) {
            this.ActivityId = activityId;
            this.SequenceNo = sequenceNo;
            this.Occurrence = occurrence;
        }
        [DataMember]
        public ActivityId ActivityId { get; set; }
        [DataMember]
        public int SequenceNo { get; set; }
        [DataMember]
        public DateTime Occurrence { get; set; }
    }

    [DataContract]
    public class ActivityEventStateChange : ActivityEvent, IActivityEventChangeStatus {
        public ActivityEventStateChange() {
        }

        public ActivityEventStateChange(
                ActivityId activityId,
                int sequenceNo,
                DateTime occurrence,
                ActivityStatus status,
                Exception? error = default
            ) : base(
                activityId,
                sequenceNo,
                occurrence
            ) {
            this.Status = status;
            this.Error = error;
        }

        [DataMember]
        public ActivityStatus Status { get; set; }

        [DataMember]
        //[IgnoreDataMember]
        public Exception? Error { get; set; }
    }

    [DataContract]
    public class ActivityEventProgress : ActivityEvent {
        public ActivityEventProgress() {
            this.Category = string.Empty;
        }
        public ActivityEventProgress(
                ActivityId activityId,
                int sequenceNo,
                DateTime occurrence,
                string category,
                int stepNo
            ) : base(
                activityId,
                sequenceNo,
                occurrence
            ) {
            this.Category = category;
            this.StepNo = stepNo;
        }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public int StepNo { get; set; }
    }

    /*
    public class ActivityEventX : ActivityEvent {
        public ActivityEventX() {
        }
        public ActivityEventX(
                Guid operationId,
                Guid executionId,
                int sequenceNo,
                DateTime occurrence
            ) : base(
                operationId,
                executionId,
                sequenceNo,
                occurrence
            ) {
        }
    }
    */
}
