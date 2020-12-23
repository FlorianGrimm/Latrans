using Brimborium.Latrans.Activity;

using System;
using System.Collections.Generic;
using System.Text;

namespace Brimborium.Latrans.Activity {
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

        public ActivityId ActivityId { get; set; }
        public int SequenceNo { get; set; }
        public DateTime Occurrence { get; set; }
    }

    public class ActivityEventStateChange : ActivityEvent, IActivityEventChangeStatus {
        public ActivityEventStateChange() {
        }

        public ActivityEventStateChange(
                ActivityId activityId,
                int sequenceNo,
                DateTime occurrence,
                ActivityStatus status
            ) : base(
                activityId,
                sequenceNo,
                occurrence
            ) {
            this.Status = status;
        }

        public ActivityStatus Status { get; set; }
    }
    public class ActivityEventProgress : ActivityEvent {
        public ActivityEventProgress() {
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

        public string Category { get; set; }

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
