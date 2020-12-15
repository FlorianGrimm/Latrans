using Brimborium.Latrans.Utility;

using Microsoft.Extensions.DependencyInjection;

using System;

namespace Brimborium.Latrans.Medaitor {
    public sealed class MedaitorAccess : IMedaitorAccess {
        private readonly IServiceProvider _ServiceProvider;

        public MedaitorAccess(IServiceProvider serviceProvider) {
            this._ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IMedaitorClient GetMedaitorClient() {
            var result = this._ServiceProvider.GetRequiredService<IMedaitorClient>();
            return result;
        }

        //public IUsingValue<IMedaitorClient> GetMedaitorClient() {
        //    return UsingValue.CreateByServiceProvider<IMedaitorClient>(this._ServiceProvider);
        //    //IMedaitorClient medaitorClient = this._ServiceProvider.GetRequiredService<IMedaitorClient>();
        //    //throw new NotImplementedException();
        //}
    }
}
