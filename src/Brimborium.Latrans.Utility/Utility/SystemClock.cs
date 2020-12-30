using Brimborium.Latrans.Contracts;

namespace Brimborium.Latrans.Utility {
    public sealed class SystemClock 
        : ISystemClock {
        public System.DateTime UtcNow => System.DateTime.UtcNow;
    }
}
