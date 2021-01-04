#nullable disable

using System;
using System.Text;
using Brimborium.Latrans.JSON.Internal;

using System.Runtime.CompilerServices;
//
namespace Brimborium.Latrans.JSON {
    public sealed class JsonWriterUtf8 : JsonWriter {
        static readonly byte[] emptyBytes = new byte[0];

        // write direct from UnsafeMemory
        internal byte[] buffer;

        internal int offset;

        public override int CurrentOffset {
            get {
                return offset;
            }
        }

        public override void AdvanceOffset(int offset) {
            this.offset += offset;
        }

        public JsonWriterUtf8() {
            this.buffer = null;
            this.offset = 0;
        }

        public JsonWriterUtf8(byte[] initialBuffer) {
            this.buffer = initialBuffer;
            this.offset = 0;
        }

        public override ArraySegment<byte> GetBuffer() {
            if (buffer == null) return new ArraySegment<byte>(emptyBytes, 0, 0);
            return new ArraySegment<byte>(buffer, 0, offset);
        }

        public override byte[] ToUtf8ByteArray() {
            if (buffer == null) return emptyBytes;
            return ByteArrayUtil.FastCloneWithResize(buffer, offset);
        }

        public override string ToString() {
            if (buffer == null) return null;
            return Encoding.UTF8.GetString(buffer, 0, offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void EnsureCapacity(int appendLength) {
            ByteArrayUtil.EnsureCapacity(ref buffer, offset, appendLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void EnsureCapacity(int offset, int appendLength) {
            ByteArrayUtil.EnsureCapacity(ref this.buffer, offset, appendLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> GetBuffer(int appendLength) {
            ByteArrayUtil.EnsureCapacity(ref this.buffer, offset, appendLength);
            return new Span<byte>(this.buffer, offset, appendLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> GetBuffer(int offset, int appendLength) { 
            ByteArrayUtil.EnsureCapacity(ref this.buffer, offset, appendLength);
            return new Span<byte>(this.buffer, offset, appendLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteRaw(byte rawValue) {
            ByteArrayUtil.EnsureCapacity(ref buffer, offset, 1);
            buffer[offset++] = rawValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteRaw(byte[] rawValue) {
#if NETSTANDARD
            UnsafeMemory.WriteRaw(this, rawValue);
#else
            BinaryUtil.EnsureCapacity(ref buffer, offset, rawValue.Length);
            Buffer.BlockCopy(rawValue, 0, buffer, offset, rawValue.Length);
            offset += rawValue.Length;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteRawUnsafe(byte rawValue) {
            buffer[offset++] = rawValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteBeginArray() {
            ByteArrayUtil.EnsureCapacity(ref buffer, offset, 1);
            buffer[offset++] = (byte)'[';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteEndArray() {
            ByteArrayUtil.EnsureCapacity(ref buffer, offset, 1);
            buffer[offset++] = (byte)']';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteBeginObject() {
            ByteArrayUtil.EnsureCapacity(ref buffer, offset, 1);
            buffer[offset++] = (byte)'{';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteEndObject() {
            ByteArrayUtil.EnsureCapacity(ref buffer, offset, 1);
            buffer[offset++] = (byte)'}';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteValueSeparator() {
            ByteArrayUtil.EnsureCapacity(ref buffer, offset, 1);
            buffer[offset++] = (byte)',';
        }

        /// <summary>:</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteNameSeparator() {
            ByteArrayUtil.EnsureCapacity(ref buffer, offset, 1);
            buffer[offset++] = (byte)':';
        }

        /// <summary>WriteString + WriteNameSeparator</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WritePropertyName(string propertyName) {
            WriteString(propertyName);
            WriteNameSeparator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteQuotation() {
            ByteArrayUtil.EnsureCapacity(ref buffer, offset, 1);
            buffer[offset++] = (byte)'\"';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteNull() {
            ByteArrayUtil.EnsureCapacity(ref buffer, offset, 4);
            buffer[offset + 0] = (byte)'n';
            buffer[offset + 1] = (byte)'u';
            buffer[offset + 2] = (byte)'l';
            buffer[offset + 3] = (byte)'l';
            offset += 4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteBoolean(bool value) {
            if (value) {
                ByteArrayUtil.EnsureCapacity(ref buffer, offset, 4);
                buffer[offset + 0] = (byte)'t';
                buffer[offset + 1] = (byte)'r';
                buffer[offset + 2] = (byte)'u';
                buffer[offset + 3] = (byte)'e';
                offset += 4;
            } else {
                ByteArrayUtil.EnsureCapacity(ref buffer, offset, 5);
                buffer[offset + 0] = (byte)'f';
                buffer[offset + 1] = (byte)'a';
                buffer[offset + 2] = (byte)'l';
                buffer[offset + 3] = (byte)'s';
                buffer[offset + 4] = (byte)'e';
                offset += 5;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteTrue() {
            ByteArrayUtil.EnsureCapacity(ref buffer, offset, 4);
            buffer[offset + 0] = (byte)'t';
            buffer[offset + 1] = (byte)'r';
            buffer[offset + 2] = (byte)'u';
            buffer[offset + 3] = (byte)'e';
            offset += 4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteFalse() {
            ByteArrayUtil.EnsureCapacity(ref buffer, offset, 5);
            buffer[offset + 0] = (byte)'f';
            buffer[offset + 1] = (byte)'a';
            buffer[offset + 2] = (byte)'l';
            buffer[offset + 3] = (byte)'s';
            buffer[offset + 4] = (byte)'e';
            offset += 5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteSingle(Single value) {
            offset += Brimborium.Latrans.JSON.Internal.DoubleConversion.DoubleToStringConverter.GetBytes(ref buffer, offset, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteDouble(double value) {
            offset += Brimborium.Latrans.JSON.Internal.DoubleConversion.DoubleToStringConverter.GetBytes(ref buffer, offset, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteByte(byte value) {
            WriteUInt64((ulong)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteUInt16(ushort value) {
            WriteUInt64((ulong)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteUInt32(uint value) {
            WriteUInt64((ulong)value);
        }

        public override void WriteUInt64(ulong value) {
            offset += NumberConverter.WriteUInt64(ref buffer, offset, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteSByte(sbyte value) {
            WriteInt64((long)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteInt16(short value) {
            WriteInt64((long)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteInt32(int value) {
            WriteInt64((long)value);
        }

        public override void WriteInt64(long value) {
            offset += NumberConverter.WriteInt64(ref buffer, offset, value);
        }

        public override void WriteString(string value) {
            if (value == null) {
                WriteNull();
                return;
            }

            // single-path escape

            // nonescaped-ensure
            var startoffset = offset;
            var max = StringEncoding.UTF8.GetMaxByteCount(value.Length) + 2;
            ByteArrayUtil.EnsureCapacity(ref buffer, startoffset, max);

            var from = 0;
            //var to = value.Length;

            buffer[offset++] = (byte)'\"';

            // for JIT Optimization, for-loop i < str.Length
            for (int i = 0; i < value.Length; i++) {
                byte escapeChar = default(byte);
                switch (value[i]) {
                    case '"':
                        escapeChar = (byte)'"';
                        break;
                    case '\\':
                        escapeChar = (byte)'\\';
                        break;
                    case '\b':
                        escapeChar = (byte)'b';
                        break;
                    case '\f':
                        escapeChar = (byte)'f';
                        break;
                    case '\n':
                        escapeChar = (byte)'n';
                        break;
                    case '\r':
                        escapeChar = (byte)'r';
                        break;
                    case '\t':
                        escapeChar = (byte)'t';
                        break;
                    // use switch jumptable
                    case (char)0:
                    case (char)1:
                    case (char)2:
                    case (char)3:
                    case (char)4:
                    case (char)5:
                    case (char)6:
                    case (char)7:
                    case (char)11:
                    case (char)14:
                    case (char)15:
                    case (char)16:
                    case (char)17:
                    case (char)18:
                    case (char)19:
                    case (char)20:
                    case (char)21:
                    case (char)22:
                    case (char)23:
                    case (char)24:
                    case (char)25:
                    case (char)26:
                    case (char)27:
                    case (char)28:
                    case (char)29:
                    case (char)30:
                    case (char)31:
                    case (char)32:
                    case (char)33:
                    case (char)35:
                    case (char)36:
                    case (char)37:
                    case (char)38:
                    case (char)39:
                    case (char)40:
                    case (char)41:
                    case (char)42:
                    case (char)43:
                    case (char)44:
                    case (char)45:
                    case (char)46:
                    case (char)47:
                    case (char)48:
                    case (char)49:
                    case (char)50:
                    case (char)51:
                    case (char)52:
                    case (char)53:
                    case (char)54:
                    case (char)55:
                    case (char)56:
                    case (char)57:
                    case (char)58:
                    case (char)59:
                    case (char)60:
                    case (char)61:
                    case (char)62:
                    case (char)63:
                    case (char)64:
                    case (char)65:
                    case (char)66:
                    case (char)67:
                    case (char)68:
                    case (char)69:
                    case (char)70:
                    case (char)71:
                    case (char)72:
                    case (char)73:
                    case (char)74:
                    case (char)75:
                    case (char)76:
                    case (char)77:
                    case (char)78:
                    case (char)79:
                    case (char)80:
                    case (char)81:
                    case (char)82:
                    case (char)83:
                    case (char)84:
                    case (char)85:
                    case (char)86:
                    case (char)87:
                    case (char)88:
                    case (char)89:
                    case (char)90:
                    case (char)91:
                    default:
                        continue;
                }

                max += 2;
                ByteArrayUtil.EnsureCapacity(ref buffer, startoffset, max); // check +escape capacity

                offset += StringEncoding.UTF8.GetBytes(value, from, i - from, buffer, offset);
                from = i + 1;
                buffer[offset++] = (byte)'\\';
                buffer[offset++] = escapeChar;
            }

            if (from != value.Length) {
                offset += StringEncoding.UTF8.GetBytes(value, from, value.Length - from, buffer, offset);
            }

            buffer[offset++] = (byte)'\"';
        }
    }
}