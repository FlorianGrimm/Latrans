using System;

namespace Brimborium.Latrans.Utility {
    public interface ILocalDisposables : IOwnedDisposable, IDisposable {
        T Add<T>(T value) where T : class, IDisposable;
    }
}
