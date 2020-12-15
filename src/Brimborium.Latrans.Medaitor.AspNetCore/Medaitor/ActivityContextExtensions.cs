using Brimborium.Latrans.Activity;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Brimborium.Latrans.Medaitor
{
    public static class ActivityContextExtensions {
        public static ActionResult<T> ReturnAsActionResult<T>(this IActivityContext activityContext) {
            throw new NotImplementedException();
        }
    }
}
