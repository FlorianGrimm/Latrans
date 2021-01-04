using System;
using System.Collections.Generic;
using System.Text;
using Brimborium.Latrans.JSON.Internal;

#if NETSTANDARD
using System.Runtime.CompilerServices;
#endif

namespace Brimborium.Latrans.JSON {
    // JSON RFC: https://www.ietf.org/rfc/rfc4627.txt

    public abstract class JsonReader {
        protected JsonReader() {
        }

#warning here
        //public abstract ArraySegment<byte> ReadNextBlockSegment();
        // public abstract ArraySegment<byte> ReadNumberSegment();
        public abstract ArraySegment<byte> ReadPropertyNameSegmentRaw();
        public abstract ArraySegment<byte> ReadStringSegmentRaw();
        public abstract ArraySegment<byte> ReadStringSegmentUnsafe();
        public abstract bool TryGetParameterValue(JsonSerializationInfo jsonSerializationInfo, out int key);
        public abstract Double ReadDouble();
        public abstract JsonToken GetCurrentJsonToken();
        public abstract Single ReadSingle();
        public abstract bool ReadBoolean();
        public abstract bool ReadIsBeginArray();
        public abstract bool ReadIsBeginObject();
        public abstract bool ReadIsEndArray();
        public abstract bool ReadIsEndArrayWithSkipValueSeparator(ref int count);
        public abstract bool ReadIsEndObject();
        public abstract bool ReadIsEndObjectWithSkipValueSeparator(ref int count);
        public abstract bool ReadIsInArray(ref int count);
        public abstract bool ReadIsInObject(ref int count);
        public abstract bool ReadIsNameSeparator();
        public abstract bool ReadIsNull();
        public abstract bool ReadIsValueSeparator();
        public abstract byte ReadByte();
        public abstract byte[] GetBufferUnsafe();
        public abstract int GetCurrentOffsetUnsafe();
        public abstract int ReadInt32();
        public abstract long ReadInt64();
        public abstract sbyte ReadSByte();
        public abstract short ReadInt16();
        public abstract string ReadPropertyName();
        public abstract string ReadString();
        public abstract uint ReadUInt32();
        public abstract ulong ReadUInt64();
        public abstract ushort ReadUInt16();
        public abstract void AdvanceOffset(int offset);
        public abstract void ReadIsBeginArrayWithVerify();
        public abstract void ReadIsBeginObjectWithVerify();
        public abstract void ReadIsEndArrayWithVerify();
        public abstract void ReadIsEndObjectWithVerify();
        public abstract void ReadIsNameSeparatorWithVerify();
        public abstract void ReadIsValueSeparatorWithVerify();
        public abstract void ReadNext();
        public abstract void ReadNextBlock();
        public abstract void SkipWhiteSpace();
    }
}
