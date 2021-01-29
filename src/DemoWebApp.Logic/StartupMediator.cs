using Microsoft.Extensions.DependencyInjection;

namespace DemoWebApp.Logic {
    public partial class StartupMediator {
        public void ConfigureServices(IServiceCollection services) {
            services.AddTransient<Brimborium.Latrans.Activity.IActivityHandler<ActivityModel.ConfigurationActivity.GetConfigurationRequest, ActivityModel.ConfigurationActivity.GetConfigurationResponse>>();
            ConfigureHandlers(services);
        }

        partial void ConfigureHandlers(IServiceCollection services);
    }
}
