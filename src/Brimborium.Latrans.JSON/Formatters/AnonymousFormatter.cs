﻿using System;

namespace Brimborium.Latrans.JSON.Formatters
{
    public sealed class AnonymousFormatter<T> : IJsonFormatter<T>
    {
        readonly JsonSerializeAction<T>? serialize;
        readonly JsonDeserializeFunc<T>? deserialize;

        public AnonymousFormatter(JsonSerializeAction<T>? serialize, JsonDeserializeFunc<T>? deserialize)
        {
            this.serialize = serialize;
            this.deserialize = deserialize;
        }

        public void Serialize(JsonWriter writer, T value, IJsonFormatterResolver formatterResolver)
        {
            if (serialize == null) throw new InvalidOperationException(this.GetType().Name + " does not support Serialize.");
            serialize(writer, value, formatterResolver);
        }

        public T Deserialize(JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (deserialize == null) throw new InvalidOperationException(this.GetType().Name + " does not support Deserialize.");
            return deserialize(reader, formatterResolver);
        }
    }
}
