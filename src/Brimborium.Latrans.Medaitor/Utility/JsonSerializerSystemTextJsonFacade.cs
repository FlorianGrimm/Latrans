using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Utility {
    public class JsonSerializerSystemTextJsonFacade : IJsonSerializerFacade {
        public JsonSerializerSystemTextJsonFacade(
            System.Text.Json.JsonSerializerOptions jsonSerializerOptions
            ) {
            this.JsonSerializerOptions = jsonSerializerOptions;
        }

        public JsonSerializerOptions JsonSerializerOptions { get; }

        public object? Deserialize(JsonSerializedData jsonSerializedData) {
            Des
#warning Deserialize  NotImplementedException
            throw new NotImplementedException();
        }

        public Task<object?> DeserializeAsync(JsonSerializedData jsonSerializedData) {
            throw new NotImplementedException();
        }

        public JsonSerializedData Serialize(object? instance) {
            throw new NotImplementedException();
        }

        public Task<JsonSerializedData> SerializeAsync(object? instance) {
            throw new NotImplementedException();
        }
    }
}
