using Brimborium.Latrans.Activity;
namespace DemoWebApp.ActivityModel.ConfigurationActivity{
    public class GetConfigurationRequest : IRequest<GetConfigurationResponse> {
        public GetConfigurationRequest() {
        }
    }

    public class GetConfigurationResponse : IResponseBase {
        public GetConfigurationResponse() {
        }

        public GetConfigurationResponse(string[] result) {
            this.Result = result;
        }

        public string[] Result { get; set; }
    }

    public class GetConfigurationActivityInvoker
        : ActivityInvoker<GetConfigurationRequest, GetConfigurationResponse> {
    }
}