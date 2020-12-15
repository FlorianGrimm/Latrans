using System;

namespace Brimborium.Latrans.Utility {
    public interface ILazyValue<out T> : IDisposable
        where T : class {
        T GetValue();
    }
}
