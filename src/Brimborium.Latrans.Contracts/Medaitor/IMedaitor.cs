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
    }

    public interface IMedaitorClient {
        
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
    public interface IMedaitorService {

    }
}
