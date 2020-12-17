using Brimborium.Latrans.Utility;

using Microsoft.Extensions.DependencyInjection;

using System;

namespace Brimborium.Latrans.Mediator {
    public sealed class MediatorAccess : IMediatorAccess {
        private readonly IServiceProvider _ServiceProvider;

        public MediatorAccess(IServiceProvider serviceProvider) {
            this._ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IMediatorClient GetMedaitorClient() {
            var result = this._ServiceProvider.GetRequiredService<IMediatorClient>();
            return result;
        }

        //public IUsingValue<IMedaitorClient> GetMedaitorClient() {
        //    return UsingValue.CreateByServiceProvider<IMedaitorClient>(this._ServiceProvider);
        //    //IMedaitorClient medaitorClient = this._ServiceProvider.GetRequiredService<IMedaitorClient>();
        //    //throw new NotImplementedException();
        //}
    }
}
