using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Mediator;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Brimborium.Latrans.Mediator {
    public static class MediatorExtensions {
        public static bool TryGetResult<T>(
                this IActivityResponse response,
                [MaybeNullWhen(false)] out T result
            ) {
            if (response is OkResultActivityResponse<T> okResult) {
                result = okResult.Result;
                return true;
            } else {
                result = default;
                return false;
            }
        }

        public static T GetResultOrDefault<T>(
                this IActivityResponse response,
                T defaultValue
            ) {
            if (response is OkResultActivityResponse<T> okResult) {
                return okResult.Result;
            } else {
                return defaultValue;
            }
        }
    }
}