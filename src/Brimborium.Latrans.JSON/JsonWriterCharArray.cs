//
using System;

namespace Brimborium.Latrans.JSON {
    public sealed class JsonWriterCharArray : JsonWriter {
        static readonly char[] emptyBytes = new char[0];

        // write direct from UnsafeMemory
        internal char[] buffer;

        internal int offset;

        public override int CurrentOffset {
            get {
                return offset;
            }
        }

        public JsonWriterCharArray(char[] buffer) {
            this.buffer = buffer;
            this.offset = 0;
        }

        public override void AdvanceOffset(int offset) {
            throw new NotImplementedException();
        }

        public override void EnsureCapacity(int offset, int appendLength) {
            throw new NotImplementedException();
        }

        public override void EnsureCapacity(int appendLength) {
            throw new NotImplementedException();
        }

        public override ArraySegment<byte> GetBuffer() {
            throw new NotImplementedException();
        }

        public override byte[] ToUtf8ByteArray() {
            throw new NotImplementedException();
        }

        public override void WriteBeginArray() {
            throw new NotImplementedException();
        }

        public override void WriteBeginObject() {
            throw new NotImplementedException();
        }

        public override void WriteBoolean(bool value) {
            throw new NotImplementedException();
        }

        public override void WriteByte(byte value) {
            throw new NotImplementedException();
        }

        public override void WriteDouble(double value) {
            throw new NotImplementedException();
        }

        public override void WriteEndArray() {
            throw new NotImplementedException();
        }

        public override void WriteEndObject() {
            throw new NotImplementedException();
        }

        public override void WriteFalse() {
            throw new NotImplementedException();
        }

        public override void WriteInt16(short value) {
            throw new NotImplementedException();
        }

        public override void WriteInt32(int value) {
            throw new NotImplementedException();
        }

        public override void WriteInt64(long value) {
            throw new NotImplementedException();
        }

        public override void WriteNameSeparator() {
            throw new NotImplementedException();
        }

        public override void WriteNull() {
            throw new NotImplementedException();
        }

        public override void WritePropertyName(string propertyName) {
            throw new NotImplementedException();
        }

        public override void WriteQuotation() {
            throw new NotImplementedException();
        }

        public override void WriteRaw(byte rawValue) {
            throw new NotImplementedException();
        }

        public override void WriteRaw(byte[] rawValue) {
            throw new NotImplementedException();
        }

        public override void WriteRawUnsafe(byte rawValue) {
            throw new NotImplementedException();
        }

        public override void WriteSByte(sbyte value) {
            throw new NotImplementedException();
        }

        public override void WriteSingle(float value) {
            throw new NotImplementedException();
        }

        public override void WriteString(string value) {
            throw new NotImplementedException();
        }

        public override void WriteTrue() {
            throw new NotImplementedException();
        }

        public override void WriteUInt16(ushort value) {
            throw new NotImplementedException();
        }

        public override void WriteUInt32(uint value) {
            throw new NotImplementedException();
        }

        public override void WriteUInt64(ulong value) {
            throw new NotImplementedException();
        }

        public override void WriteValueSeparator() {
            throw new NotImplementedException();
        }
    }
}