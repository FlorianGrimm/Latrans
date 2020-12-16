namespace Brimborium.Latrans.Activity {
    public interface IActivityResponse {
        IActivityEvent GetAsActivityEvent(IActivityContext activityContext);
    }

    // public interface IFinalActivityResponse : IActivityResponse {    }

    public interface IFailureActivityResponse : IActivityResponse {
        System.Exception GetError();
    }
}
