using Microsoft.Extensions.Options;

using System;

namespace Brimborium.Latrans.Mediator {
    public class ActivityExecutionConfigurationDefaults {
        private readonly string? _RedirectUrlBase;
        private readonly Func<string, Guid, string> _GetRedirectUrl;

        public ActivityExecutionConfigurationDefaults(
            IOptions<ActivityExecutionConfigurationOptions> options
            ) {
            var opt = options.Value;
            this._RedirectUrlBase = opt.RedirectUrlBase;
            this._GetRedirectUrl = opt.GetRedirectUrl;
        }

        private ActivityExecutionConfiguration? _ForQueryCancelable;
        public ActivityExecutionConfiguration ForQueryCancelable
            => _ForQueryCancelable ??= new ActivityExecutionConfiguration(
                    TimeSpan.FromSeconds(30),
                    true,
                    false,
                    this._RedirectUrlBase,
                    this._GetRedirectUrl
                );

        private ActivityExecutionConfiguration? _ForChanges;
        public ActivityExecutionConfiguration ForChanges
            => _ForChanges ??= new ActivityExecutionConfiguration(
                    TimeSpan.MaxValue,
                    false,
                    false,
                    this._RedirectUrlBase,
                    this._GetRedirectUrl
                );

        private ActivityExecutionConfiguration? _ForLongRunning;
        public ActivityExecutionConfiguration ForLongRunning
            => _ForLongRunning ??= new ActivityExecutionConfiguration(
                    TimeSpan.FromSeconds(10),
                    false,
                    true,
                    this._RedirectUrlBase,
                    this._GetRedirectUrl
                );
    }
}
