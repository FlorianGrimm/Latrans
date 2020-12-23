using System;

namespace Brimborium.Latrans.Activity {
    public interface IActivityEvent {
        ActivityId ActivityId { get; set; }

        int SequenceNo { get; set; }

        DateTime Occurrence { get; set; }
    }

    public interface IActivityEventChangeStatus: IActivityEvent {
        ActivityStatus Status { get; set; }
    }
}
