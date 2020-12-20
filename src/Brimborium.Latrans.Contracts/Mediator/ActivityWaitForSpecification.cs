
using System;

namespace Brimborium.Latrans.Mediator {

    public class ActivityWaitForSpecification {
        private static ActivityWaitForSpecification? _Default;
        public static ActivityWaitForSpecification Default
            => _Default ??= new ActivityWaitForSpecification();

        public ActivityWaitForSpecification() {
            this.WaitTimeSpan = TimeSpan.MaxValue;
            this.RespectRequestAborted = false;
            this.SupportAccepted202Redirect = false;
        }

        public ActivityWaitForSpecification(
            TimeSpan waitTimeSpan,
            bool respectRequestAborted,
            bool supportAccepted202Redirect,
            string? redirectUrlBase,
            Func<string, Guid, string> getRedirectUrl
            ) {
            this.WaitTimeSpan = waitTimeSpan;
            this.RespectRequestAborted = respectRequestAborted;
            this.SupportAccepted202Redirect = supportAccepted202Redirect;
            this.RedirectUrlBase = redirectUrlBase;
            this.GetRedirectUrl = getRedirectUrl;
        }

        public TimeSpan WaitTimeSpan { get; }
        public bool RespectRequestAborted { get; }
        public bool SupportAccepted202Redirect { get; }
        public string? RedirectUrlBase { get; }
        public Func<string, Guid, string>? GetRedirectUrl { get; }

        public string RedirectUrl(Guid id) {
            if (this.GetRedirectUrl is null) {
                throw new NotSupportedException("GetRedirectUrl is null.");
            }
            if (string.IsNullOrEmpty(this.RedirectUrlBase)) {
                throw new NotSupportedException("GetRedirectUrl is empty.");
            }
            return this.GetRedirectUrl(this.RedirectUrlBase, id);
        }

        public static string DefaultGetRedirectUrl(string redirectUrlBase, Guid id) {
            return $"{redirectUrlBase}/{id}";
        }
    }
}
