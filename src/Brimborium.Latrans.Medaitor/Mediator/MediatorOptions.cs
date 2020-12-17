
using Microsoft.Extensions.DependencyInjection;

namespace Brimborium.Latrans.Mediator {
    public class MediatorOptions {
        public readonly IServiceCollection ServicesMediator;
        public readonly RequestRelatedTypes RequestRelatedTypes;

        public MediatorOptions(
            IServiceCollection services,
            RequestRelatedTypes requestRelatedTypes
            ) {
            this.RequestRelatedTypes = requestRelatedTypes??new RequestRelatedTypes();
            this.ServicesMediator = services??new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        }
    }
}
