using System;

namespace Brimborium.Latrans.Utility {
    public interface ILocalDisposables : IDisposable {
        T Add<T>(T value) where T : class, IDisposable;
    }
}
