using Brimborium.Latrans.Contracts.Medaitor;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Medaitor
{
    public class MedaitorClient : IMedaitorClient {
        public Task ExecuteAsync(CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
    }
    public class MedaitorService : IMedaitorService {
    }
}
