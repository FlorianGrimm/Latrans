using System.Threading.Tasks;

using Brimborium.Latrans.Activity;

namespace Brimborium.Latrans.Mediator {
    public class MediatorServiceStorageNull : IMediatorServiceStorage {
        public Task AddActivityEventAsync(IActivityEvent activityEvent) {
            return Task.CompletedTask;
        }

        public void Dispose() {
        }
    }
}
