using System.Diagnostics.CodeAnalysis;

namespace Brimborium.Latrans.Activity {
    public interface IActivityResponse {
        IActivityEvent GetAsActivityEvent(IActivityContext activityContext);
    }

    // public interface IFinalActivityResponse : IActivityResponse {    }

    public interface IOkResultActivityResponse : IActivityResponse {
        bool TryGetResult([MaybeNullWhen(false)]out object value);
    }

    public interface IFailureActivityResponse : IActivityResponse {
        System.Exception? GetError();
    }
}
