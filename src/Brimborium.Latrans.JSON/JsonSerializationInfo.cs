using System.Collections.Generic;

namespace Brimborium.Latrans.JSON {
    public class JsonSerializationInfoBuilder {
        private readonly List<JsonPropertySerializationData> _Properties;

        public JsonSerializationInfoBuilder() {
            this._Properties = new List<JsonPropertySerializationData>();
        }
        public List<JsonPropertySerializationData> Properties => this._Properties;

        public JsonSerializationInfoBuilder Add(JsonPropertySerializationData info) {
            this._Properties.Add(info);
            return this;
        }

        public JsonSerializationInfoBuilder Add(string name, int order, bool isReadable, bool isWritable) {
            this._Properties.Add(new JsonPropertySerializationData(
                name,
                order,
                isReadable,
                isWritable
            ));
            return this;
        }

        public JsonSerializationInfo Build() {
            return new JsonSerializationInfo(this);
        }
    }

    public class JsonPropertySerializationData {
        public JsonPropertySerializationData(
                string name,
                int order,
                bool isReadable,
                bool isWritable
            ) {
            this.Name = name;
            this.Order = order;
            this.IsReadable = isReadable;
            this.IsWritable = isWritable;
        }

        public string Name { get; set; }
        public int Order { get; set; }
        public bool IsReadable { get; set; }
        public bool IsWritable { get; set; }
    }


    public class JsonSerializationInfo {
        public JsonSerializationInfo(JsonSerializationInfoBuilder builder) {
        }
    }
    public class JsonPropertySerializationInfo {
        public JsonPropertySerializationInfo(
                string name,
                int order,
                bool isReadable,
                bool isWritable
            ) {
            this.Name = name;
            this.Order = order;
            this.IsReadable = isReadable;
            this.IsWritable = isWritable;
        }

        public string Name { get; }
        public int Order { get; }
        public bool IsReadable { get; }
        public bool IsWritable { get; }
    }

}