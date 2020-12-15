using Brimborium.Latrans.Utility;

using System;

namespace Brimborium.Latrans.Medaitor {
    public sealed class MedaitorAccess : IMedaitorAccess {
        private readonly IServiceProvider _ServiceProvider;

        public MedaitorAccess(IServiceProvider serviceProvider) {
            this._ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IUsingValue<IMedaitorClient> GetMedaitorClient() {
            return UsingValue.CreateByServiceProvider<IMedaitorClient>(this._ServiceProvider);
            //IMedaitorClient medaitorClient = this._ServiceProvider.GetRequiredService<IMedaitorClient>();
            //throw new NotImplementedException();
        }
    }
}
