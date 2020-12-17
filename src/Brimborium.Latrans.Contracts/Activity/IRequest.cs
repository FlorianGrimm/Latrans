using System;

namespace Brimborium.Latrans.Activity {
    public interface IRequestBase {
    }

    public interface IResponseBase {
    }

    public interface IRequest<out TResponse> : IRequestBase
       where TResponse : IResponseBase {
    }

}

