using System;

namespace Brimborium.Latrans.Utility {
    public interface IUsingValue<out T> : IDisposable
        where T : class, IDisposable {
        T GetValue();
    }
}
