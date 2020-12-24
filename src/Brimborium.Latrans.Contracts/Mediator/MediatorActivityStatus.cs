using Brimborium.Latrans.Activity;

using System;

namespace Brimborium.Latrans.Mediator {
    public class MediatorActivityStatus {
        public ActivityStatus Status { get; set; }

        public ActivityId ActivityId { get; set; }

        public IActivityEvent[] ActivityEvents { get; set; }

        public MediatorActivityStatus() {
            this.Status = ActivityStatus.Initialize;
            this.ActivityId = ActivityId.Empty();
            this.ActivityEvents = Array.Empty<IActivityEvent>();
        }
    }
}
