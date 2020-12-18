﻿
using Microsoft.Extensions.Options;

using System;

namespace Brimborium.Latrans.Mediator {

    public class ActivityWaitForSpecification {
        private static ActivityWaitForSpecification _Default;
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
            string redirectUrlBase,
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
        public string RedirectUrlBase { get; }
        public Func<string, Guid, string> GetRedirectUrl { get; }

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

    public class ActivityWaitForSpecificationOptions {
        public ActivityWaitForSpecificationOptions() {
            this.GetRedirectUrl = ActivityWaitForSpecification.DefaultGetRedirectUrl;
        }
        public string RedirectUrlBase { get; set; }
        public Func<string, Guid, string> GetRedirectUrl { get; set; }
    }

    public class ActivityWaitForSpecificationDefaults {
        private readonly string _RedirectUrlBase;
        private readonly Func<string, Guid, string> _GetRedirectUrl;

        public ActivityWaitForSpecificationDefaults(
            IOptions<ActivityWaitForSpecificationOptions> options
            ) {
            var opt = options.Value;
            this._RedirectUrlBase = opt.RedirectUrlBase;
            this._GetRedirectUrl = opt.GetRedirectUrl;
        }
        private ActivityWaitForSpecification _ForQueryCancelable;
        public ActivityWaitForSpecification ForQueryCancelable
            => _ForQueryCancelable ??= new ActivityWaitForSpecification(
                    TimeSpan.FromSeconds(30),
                    true,
                    false,
                    this._RedirectUrlBase,
                    this._GetRedirectUrl
                );

        private ActivityWaitForSpecification _ForChanges;
        public ActivityWaitForSpecification ForChanges
            => _ForChanges ??= new ActivityWaitForSpecification(
                    TimeSpan.MaxValue,
                    false,
                    false,
                    this._RedirectUrlBase,
                    this._GetRedirectUrl
                );

        private ActivityWaitForSpecification _ForLongRunning;
        

        public ActivityWaitForSpecification ForLongRunning
            => _ForLongRunning ??= new ActivityWaitForSpecification(
                    TimeSpan.FromSeconds(10),
                    false,
                    true,
                    this._RedirectUrlBase,
                    this._GetRedirectUrl
                );
    }
}
