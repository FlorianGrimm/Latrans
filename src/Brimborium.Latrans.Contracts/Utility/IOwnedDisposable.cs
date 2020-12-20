using System;

namespace Brimborium.Latrans.Utility {
    public interface IOwnedDisposable : IDisposable {
        void Dispose(object owner);
    }
}
