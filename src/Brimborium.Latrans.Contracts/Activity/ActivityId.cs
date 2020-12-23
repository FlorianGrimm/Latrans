using System;

namespace Brimborium.Latrans.Activity {
    public struct ActivityId {
        public static ActivityId NewId()
            => new ActivityId(Guid.NewGuid(), Guid.NewGuid());

        public Guid OperationId;
        public Guid ExecutionId;
        public ActivityId(
                Guid operationId,
                Guid executionId
            ) {
            this.OperationId = operationId;
            this.ExecutionId = executionId;
        }
    }
}
