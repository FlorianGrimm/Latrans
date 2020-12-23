
using System;

namespace Brimborium.Latrans.Mediator {
    public class ActivityExecutionConfigurationOptions {
        public ActivityExecutionConfigurationOptions() {
            this.GetRedirectUrl = ActivityExecutionConfiguration.DefaultGetRedirectUrl;
        }
        public string? RedirectUrlBase { get; set; }
        public Func<string, Guid, string> GetRedirectUrl { get; set; }
    }

}
