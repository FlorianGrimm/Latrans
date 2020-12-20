
using Microsoft.Extensions.Options;

using System;

namespace Brimborium.Latrans.Mediator {
    public class ActivityWaitForSpecificationDefaults {
        private readonly string? _RedirectUrlBase;
        private readonly Func<string, Guid, string> _GetRedirectUrl;

        public ActivityWaitForSpecificationDefaults(
            IOptions<ActivityWaitForSpecificationOptions> options
            ) {
            var opt = options.Value;
            this._RedirectUrlBase = opt.RedirectUrlBase;
            this._GetRedirectUrl = opt.GetRedirectUrl;
        }
        private ActivityWaitForSpecification? _ForQueryCancelable;
        public ActivityWaitForSpecification ForQueryCancelable
            => _ForQueryCancelable ??= new ActivityWaitForSpecification(
                    TimeSpan.FromSeconds(30),
                    true,
                    false,
                    this._RedirectUrlBase,
                    this._GetRedirectUrl
                );

        private ActivityWaitForSpecification? _ForChanges;
        public ActivityWaitForSpecification ForChanges
            => _ForChanges ??= new ActivityWaitForSpecification(
                    TimeSpan.MaxValue,
                    false,
                    false,
                    this._RedirectUrlBase,
                    this._GetRedirectUrl
                );

        private ActivityWaitForSpecification? _ForLongRunning;
        public ActivityWaitForSpecification ForLongRunning
            => _ForLongRunning ??= new ActivityWaitForSpecification(
                    TimeSpan.FromSeconds(10),
                    false,
                    true,
                    this._RedirectUrlBase,
                    this._GetRedirectUrl
                );
    }

    public class ActivityWaitForSpecificationOptions {
        public ActivityWaitForSpecificationOptions() {
            this.GetRedirectUrl = ActivityWaitForSpecification.DefaultGetRedirectUrl;
        }
        public string? RedirectUrlBase { get; set; }
        public Func<string, Guid, string> GetRedirectUrl { get; set; }
    }

}
