using Microsoft.Extensions.DependencyInjection;

using System;
using System.Diagnostics.CodeAnalysis;

namespace Brimborium.Latrans.Utility {
    public class DisposedServiceProvider : IServiceProvider, IDisposable {
        private static DisposedServiceProvider? _Instance;

        [NotNull]
        public static DisposedServiceProvider Instance
            => _Instance ??= new DisposedServiceProvider();

        public DisposedServiceProvider() {
        }

        public object GetService(Type serviceType) => throw new ObjectDisposedException("ServiceProvider");

        public void Dispose() { }

        public override bool Equals(object? obj)
            => (obj is null) ? true : base.Equals(obj);

        public override int GetHashCode() => 1;
    }

    public class DisposedServiceScope : IServiceScope {
        private static DisposedServiceScope? _Instance;

        [NotNull]
        public static DisposedServiceScope Instance
            => _Instance ??= new DisposedServiceScope();

        public DisposedServiceScope() {

        }
        public IServiceProvider ServiceProvider => throw new ObjectDisposedException("ServiceProvider");
        public void Dispose() { }

        public override bool Equals(object? obj)
            => (obj is null) ? true : base.Equals(obj);

        public override int GetHashCode() => 1;
    }
}
