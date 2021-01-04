using System;

namespace Brimborium.Latrans.JSON {
    public delegate void JsonSerializeAction<T>(JsonWriter writer, T value, IJsonFormatterResolver resolver);
    public delegate T JsonDeserializeFunc<T>(JsonReader reader, IJsonFormatterResolver resolver);

    public interface IJsonFormatter {
    }

    public interface IJsonFormatter<T> : IJsonFormatter {
        void Serialize(JsonWriter writer, T value, IJsonFormatterResolver formatterResolver);

        T Deserialize(JsonReader reader, IJsonFormatterResolver formatterResolver);
    }

    public interface IJsonFormatterSpecWriter<T, TJsonWriter>
        : IJsonFormatter
        where TJsonWriter : JsonWriter{
        void SerializeSpec(TJsonWriter writer, T value, IJsonFormatterResolver formatterResolver);
    }
    public interface IJsonFormatterSpecReader<T, TJsonReader>
        : IJsonFormatter
        where TJsonReader : JsonReader {
        T DeserializeSpec(TJsonReader reader, IJsonFormatterResolver formatterResolver);
    }

    public interface IObjectPropertyNameFormatter<T> {
        void SerializeToPropertyName(JsonWriter writer, T value, IJsonFormatterResolver formatterResolver);
        T DeserializeFromPropertyName(JsonReader reader, IJsonFormatterResolver formatterResolver);
    }

    public interface IObjectPropertyNameFormatterRW<T, TJsonWriter, TJsonReader>
        where TJsonWriter : JsonWriter
        where TJsonReader : JsonReader {
        void SerializeToPropertyNameRW(TJsonWriter writer, T value, IJsonFormatterResolver formatterResolver);
        T DeserializeFromPropertyNameRW(TJsonReader reader, IJsonFormatterResolver formatterResolver);
    }


    public static class JsonFormatterExtensions {
        public static string ToJsonString<T>(this IJsonFormatter<T> formatter, T value, IJsonFormatterResolver formatterResolver) {
            var writer = new JsonWriterUtf8();
            formatter.Serialize(writer, value, formatterResolver);
            return writer.ToString();
        }
    }
}