namespace Brimborium.Latrans.Activity {
    public interface IActivityResponse {    }

    // public interface IFinalActivityResponse : IActivityResponse {    }

    public interface IFailureActivityResponse : IActivityResponse {
        System.Exception GetError();
    }
}
