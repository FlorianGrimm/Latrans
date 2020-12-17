using Brimborium.Latrans.Utility;

using Microsoft.Extensions.DependencyInjection;

using System;

namespace Brimborium.Latrans.Mediator {
    public sealed class MediatorClientFactory : IMediatorClientFactory {
        private readonly IServiceProvider _ServiceProvider;
        private readonly ILocalDisposables _LocalDisposables;

        public MediatorClientFactory(
            IServiceProvider serviceProvider,
            ILocalDisposables localDisposables
            ) {
            this._ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this._LocalDisposables = localDisposables ?? throw new ArgumentNullException(nameof(localDisposables));
            //this._Factory = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateFactory
        }

        /// <summary>
        /// creates a <see cref="MediatorClient"/> as an implementaion of <see cref="IMediatorClient"/>
        /// </summary>
        /// <returns></returns>
        public IMediatorClient GetMedaitorClient() {
            //var medaitorService = this._ServiceProvider.GetRequiredService<IMediatorService>();
            //var result = new MediatorClient(medaitorService, serviceProvider);
            // 
            var result = this._ServiceProvider.GetRequiredService<IMediatorClient>();
            this._LocalDisposables.Add(result);
            return result;
        }

        //public IUsingValue<IMedaitorClient> GetMedaitorClient() {
        //    return UsingValue.CreateByServiceProvider<IMedaitorClient>(this._ServiceProvider);
        //    //IMedaitorClient medaitorClient = this._ServiceProvider.GetRequiredService<IMedaitorClient>();
        //    //throw new NotImplementedException();
        //}
    }
}
