using Brimborium.Latrans.Mediator;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Medaitor.Controllers {
    // [Route("api/[controller]")]
    // [ApiController]
    public class MedaitorControllerBase : ControllerBase {
        protected MedaitorControllerBase() {
        }

        [NonAction]
        public IMediatorClient GetMedaitorClient() {
            var requestServices = this.HttpContext?.RequestServices;
            var mediatorClientFactory = requestServices?.GetService<IMediatorClientFactory>();
            return mediatorClientFactory?.GetMedaitorClient();
        }

        [NonAction]
        public ActivityWaitForSpecificationDefaults GetActivityWaitForSpecificationDefaults() {
            var requestServices = this.HttpContext?.RequestServices;
            return requestServices?.GetService<ActivityWaitForSpecificationDefaults>();
        }
    }
}
//namespace Brimborium.Latrans.Medaitor {
//    public static class ControllerBaseExtensions {
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static IMediatorClient GetMedaitorClient<TController>(
//                this TController that
//            )
//            where TController : ControllerBase {
//            if (that is null) { throw new ArgumentNullException(nameof(that)); }

//            var requestServices = that.HttpContext?.RequestServices;
//            var mediatorClientFactory = requestServices.GetRequiredService<IMediatorClientFactory>();
//            return mediatorClientFactory.GetMedaitorClient();
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static ActivityWaitForSpecificationDefaults GetActivityWaitForSpecificationDefaults<TController>(
//                this TController that
//            )
//            where TController : ControllerBase {
//            if (that is null) { throw new ArgumentNullException(nameof(that)); }

//            var requestServices = that.HttpContext?.RequestServices;
//            return requestServices.GetRequiredService<ActivityWaitForSpecificationDefaults>();
//        }
//    }
//}