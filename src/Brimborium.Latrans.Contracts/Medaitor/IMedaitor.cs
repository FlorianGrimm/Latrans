using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Medaitor {
    public interface IMedaitorContext {
    }
    public interface IMedaitorContext<TRequset, TResponse>: IMedaitorContext {
    }
    public interface IMedaitorHandler<TRequset, TResponse> {
        Task ExecuteAsync(
            IMedaitorContext<TRequset, TResponse> medaitorContext, 
            CancellationToken cancellationToken
            );
    }

    public interface IMedaitorClient {

        IMedaitorContext<TRequset, TResponse> CreateContext<TRequset, TResponse>(
            );

        Task ExecuteAsync<TRequset, TResponse>(
            IMedaitorContext<TRequset, TResponse> medaitorContext,
            CancellationToken cancellationToken
            );
    }
    public interface IMedaitorService {

    }
}
