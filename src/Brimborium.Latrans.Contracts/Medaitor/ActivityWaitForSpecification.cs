
using System;

namespace Brimborium.Latrans.Medaitor {
    public class ActivityWaitForSpecification {
        public ActivityWaitForSpecification() {
            this.WaitTimeSpan = TimeSpan.MaxValue;
        }

        public ActivityWaitForSpecification(TimeSpan waitTimeSpan) {
            this.WaitTimeSpan = waitTimeSpan;
        }

        public TimeSpan WaitTimeSpan { get; }
    }
}
