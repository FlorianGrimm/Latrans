using Brimborium.Latrans.Utility;

using Microsoft.Extensions.DependencyInjection;

using System;

namespace Brimborium.Latrans.Mediator {
    public sealed class MediatorClientFactory : IMediatorClientFactory {
        // ScopedServiceProvider Web
        private readonly IServiceProvider _ServiceProvider;
        // ScopedServiceProvider Web
        private readonly ILocalDisposables _LocalDisposables;

        public MediatorClientFactory(
            IServiceProvider serviceProvider,
            ILocalDisposables localDisposables
            ) {
            this._ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this._LocalDisposables = localDisposables ?? throw new ArgumentNullException(nameof(localDisposables));
        }

        /// <summary>
        /// creates a <see cref="MediatorClient"/> as an implementaion of <see cref="IMediatorClient"/>
        /// </summary>
        /// <returns></returns>
        public IMediatorClient GetMedaitorClient() {
            var result = this._ServiceProvider.GetRequiredService<IMediatorClient>();
            this._LocalDisposables.Add(result);
            return result;
        }
    }
}
