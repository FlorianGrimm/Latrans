#if NETSTANDARD
#endif

using System;

namespace Brimborium.Latrans.JSON {
    public class JsonReaderCharArray : JsonReader {
        private readonly char[] bytes;
        private int offset;

        public JsonReaderCharArray() {
            this.bytes = Array.Empty<char>();
            this.offset = 0;
        }

        public override void AdvanceOffset(int offset) {
            throw new NotImplementedException();
        }

        public override byte[] GetBufferUnsafe() {
            throw new NotImplementedException();
        }

        public override JsonToken GetCurrentJsonToken() {
            throw new NotImplementedException();
        }

        public override int GetCurrentOffsetUnsafe() {
            throw new NotImplementedException();
        }

        public override bool ReadBoolean() {
            throw new NotImplementedException();
        }

        public override byte ReadByte() {
            throw new NotImplementedException();
        }

        public override double ReadDouble() {
            throw new NotImplementedException();
        }

        public override short ReadInt16() {
            throw new NotImplementedException();
        }

        public override int ReadInt32() {
            throw new NotImplementedException();
        }

        public override long ReadInt64() {
            throw new NotImplementedException();
        }

        public override bool ReadIsBeginArray() {
            throw new NotImplementedException();
        }

        public override void ReadIsBeginArrayWithVerify() {
            throw new NotImplementedException();
        }

        public override bool ReadIsBeginObject() {
            throw new NotImplementedException();
        }

        public override void ReadIsBeginObjectWithVerify() {
            throw new NotImplementedException();
        }

        public override bool ReadIsEndArray() {
            throw new NotImplementedException();
        }

        public override bool ReadIsEndArrayWithSkipValueSeparator(ref int count) {
            throw new NotImplementedException();
        }

        public override void ReadIsEndArrayWithVerify() {
            throw new NotImplementedException();
        }

        public override bool ReadIsEndObject() {
            throw new NotImplementedException();
        }

        public override bool ReadIsEndObjectWithSkipValueSeparator(ref int count) {
            throw new NotImplementedException();
        }

        public override void ReadIsEndObjectWithVerify() {
            throw new NotImplementedException();
        }

        public override bool ReadIsInArray(ref int count) {
            throw new NotImplementedException();
        }

        public override bool ReadIsInObject(ref int count) {
            throw new NotImplementedException();
        }

        public override bool ReadIsNameSeparator() {
            throw new NotImplementedException();
        }

        public override void ReadIsNameSeparatorWithVerify() {
            throw new NotImplementedException();
        }

        public override bool ReadIsNull() {
            throw new NotImplementedException();
        }

        public override bool ReadIsValueSeparator() {
            throw new NotImplementedException();
        }

        public override void ReadIsValueSeparatorWithVerify() {
            throw new NotImplementedException();
        }

        public override void ReadNext() {
            throw new NotImplementedException();
        }

        public override void ReadNextBlock() {
            throw new NotImplementedException();
        }

        public override ArraySegment<byte> ReadNextBlockSegment() {
            throw new NotImplementedException();
        }

        public override ArraySegment<byte> ReadNumberSegment() {
            throw new NotImplementedException();
        }

        public override string ReadPropertyName() {
            throw new NotImplementedException();
        }

        public override ArraySegment<byte> ReadPropertyNameSegmentRaw() {
            throw new NotImplementedException();
        }

        public override sbyte ReadSByte() {
            throw new NotImplementedException();
        }

        public override float ReadSingle() {
            throw new NotImplementedException();
        }

        public override string ReadString() {
            throw new NotImplementedException();
        }

        public override ArraySegment<byte> ReadStringSegmentRaw() {
            throw new NotImplementedException();
        }

        public override ArraySegment<byte> ReadStringSegmentUnsafe() {
            throw new NotImplementedException();
        }

        public override ushort ReadUInt16() {
            throw new NotImplementedException();
        }

        public override uint ReadUInt32() {
            throw new NotImplementedException();
        }

        public override ulong ReadUInt64() {
            throw new NotImplementedException();
        }

        public override void SkipWhiteSpace() {
            throw new NotImplementedException();
        }
    }
}
