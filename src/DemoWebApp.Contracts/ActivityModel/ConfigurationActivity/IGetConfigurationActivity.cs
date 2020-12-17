using Brimborium.Latrans.Activity;
namespace DemoWebApp.ActivityModel.ConfigurationActivity{
    public class GetConfigurationRequest : IRequest<GetConfigurationResponse> {

    }

    public class GetConfigurationResponse : IResponseBase {
        public string[] Result { get; set; }
    }

    public interface IGetConfigurationActivity {

    }
}