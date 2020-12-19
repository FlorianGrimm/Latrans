using Brimborium.Latrans.Activity;

using System;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Mediator {
    public interface IMediatorServiceStorage : IDisposable {
        Task AddActivityEventAsync(IActivityEvent activityEvent);
    }
}
