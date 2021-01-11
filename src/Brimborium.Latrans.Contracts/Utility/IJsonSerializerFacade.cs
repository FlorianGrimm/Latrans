using System.Threading.Tasks;

namespace Brimborium.Latrans.Utility {
    public interface IJsonSerializerFacade {
        JsonSerializedData Serialize(object? instance);
        Task<JsonSerializedData> SerializeAsync(object? instance);

        object? Deserialize(JsonSerializedData jsonSerializedData);
        Task<object?> DeserializeAsync(JsonSerializedData jsonSerializedData);
    }

    public struct JsonSerializedData {
        public string? TypeName;
        public byte[]? DataByte;
    }
}
