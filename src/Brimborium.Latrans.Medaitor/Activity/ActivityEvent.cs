using Brimborium.Latrans.Activity;

using System;
using System.Collections.Generic;
using System.Text;

namespace Brimborium.Latrans.Activity {
    public class ActivityEvent : IActivityEvent {
        public ActivityEvent() {
        }
        public ActivityEvent(
            Guid operationId,
            Guid executionId,
            int sequenceNo,
            DateTime occurrence
        ) {
            this.OperationId = operationId;
            this.ExecutionId = ExecutionId;
            this.SequenceNo = sequenceNo;
            this.Occurrence = occurrence;
        }

        public Guid OperationId { get; set; }
        public Guid ExecutionId { get; set; }
        public int SequenceNo { get; set; }
        public DateTime Occurrence { get; set; }
    }

    public class ActivityEventStateChange : ActivityEvent, IActivityEventChangeStatus {
        public ActivityEventStateChange() {
        }

        public ActivityEventStateChange(
                Guid operationId,
                Guid executionId,
                int sequenceNo,
                DateTime occurrence,
                ActivityStatus status
            ) : base(
                operationId,
                executionId,
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
                Guid operationId,
                Guid executionId,
                int sequenceNo,
                DateTime occurrence,
                string category,
                int stepNo
            ) : base(
                operationId,
                executionId,
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
