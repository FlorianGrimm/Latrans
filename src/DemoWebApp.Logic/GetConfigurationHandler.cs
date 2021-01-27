using Brimborium.Latrans.Activity;

using DemoWebApp.ActivityModel.ConfigurationActivity;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace DemoWebApp.Logic {
    public class GetConfigurationHandler : ActivityHandlerBase<GetConfigurationRequest, GetConfigurationResponse> {
        public override async Task ExecuteAsync(
            IActivityContext<GetConfigurationRequest> activityContext,
            CancellationToken cancellationToken) {
            try {
                var result = new GetConfigurationResponse();
                result.Result = new string[] { "A", "B" };
                await this.SetResponseAsync(activityContext, result);
            } catch (System.Exception error) {
                await this.SetFailureResponseAsync(activityContext, error);
            }
        }
    }
}
