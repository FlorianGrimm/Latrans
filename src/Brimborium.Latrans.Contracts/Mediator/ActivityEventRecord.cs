using Brimborium.Latrans.Activity;

using System;

namespace Brimborium.Latrans.Mediator {
    public class ActivityEventKey {
        public Guid OperationId;
        public Guid ExecutionId;
        public int SequenceNo;
    }

    public class ActivityEventRecord {
        public ActivityEventRecordKind RecordKind;
        public Guid OperationId;
        public Guid ExecutionId;
        public int SequenceNo;
        public DateTime Occurrence;
        public ActivityStatus Status;
        public string? User;
        public string? Data;
    }

    public enum ActivityEventRecordKind {
        A,
        Log
    }

    public interface IActivityEventRecordConverter {
        ActivityEventRecordKind GetUsedFor();
        ActivityEventRecord Serialize(IActivityEvent activityEvent);
        IActivityEvent Deserialize(ActivityEventRecord activityEventRecord);
    }
}
