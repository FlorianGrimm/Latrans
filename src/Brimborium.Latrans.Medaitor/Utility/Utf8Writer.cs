using System;
using System.Diagnostics;
using System.Text;

namespace Brimborium.Latrans.Utility {
    public struct Utf8Writer {
        private static Encoding? _UTF8;

        public byte[] Buffer;
        public int Offset;

        public Span<byte> AsSpan() => new Span<byte>(Buffer, 0, Offset);

        public Utf8Writer(byte[]? buffer=null) {
            this.Buffer = buffer ?? MemoryPool.GetBuffer();
            this.Offset = 0;
        }

        public byte[] ToUtf8ByteArray() {
            if (Buffer == null) {
                return Array.Empty<byte>();
            } else {
                return BinaryUtil.FastCloneWithResize(Buffer, Offset);
            }
        }

        public override string ToString() {
            if (Buffer == null) {
                return string.Empty;
            } else {
                return Encoding.UTF8.GetString(Buffer, 0, Offset);
            }
        }


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureCapacity(int appendLength) {
            BinaryUtil.EnsureCapacity(ref Buffer, Offset, appendLength);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteRaw(byte rawValue) {
            BinaryUtil.EnsureCapacity(ref Buffer, Offset, 1);
            Buffer[Offset++] = rawValue;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteRaw(byte[] rawValue) {
#if false
            UnsafeMemory.WriteRaw(ref this, rawValue);
#else
            BinaryUtil.EnsureCapacity(ref Buffer, Offset, rawValue.Length);
            System.Buffer.BlockCopy(rawValue, 0, this.Buffer, this.Offset, rawValue.Length);
            Offset += rawValue.Length;
#endif
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteRawUnsafe(byte rawValue) {
            Buffer[Offset++] = rawValue;
        }


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteNull() {
            BinaryUtil.EnsureCapacity(ref Buffer, Offset, 4);
            Buffer[Offset + 0] = (byte)'n';
            Buffer[Offset + 1] = (byte)'u';
            Buffer[Offset + 2] = (byte)'l';
            Buffer[Offset + 3] = (byte)'l';
            Offset += 4;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBoolean(bool value) {
            if (value) {
                BinaryUtil.EnsureCapacity(ref Buffer, Offset, 4);
                Buffer[Offset + 0] = (byte)'t';
                Buffer[Offset + 1] = (byte)'r';
                Buffer[Offset + 2] = (byte)'u';
                Buffer[Offset + 3] = (byte)'e';
                Offset += 4;
            } else {
                BinaryUtil.EnsureCapacity(ref Buffer, Offset, 5);
                Buffer[Offset + 0] = (byte)'f';
                Buffer[Offset + 1] = (byte)'a';
                Buffer[Offset + 2] = (byte)'l';
                Buffer[Offset + 3] = (byte)'s';
                Buffer[Offset + 4] = (byte)'e';
                Offset += 5;
            }
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTrue() {
            BinaryUtil.EnsureCapacity(ref Buffer, Offset, 4);
            Buffer[Offset + 0] = (byte)'t';
            Buffer[Offset + 1] = (byte)'r';
            Buffer[Offset + 2] = (byte)'u';
            Buffer[Offset + 3] = (byte)'e';
            Offset += 4;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteFalse() {
            BinaryUtil.EnsureCapacity(ref Buffer, Offset, 5);
            Buffer[Offset + 0] = (byte)'f';
            Buffer[Offset + 1] = (byte)'a';
            Buffer[Offset + 2] = (byte)'l';
            Buffer[Offset + 3] = (byte)'s';
            Buffer[Offset + 4] = (byte)'e';
            Offset += 5;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSingle(Single value) {
            Offset += Utf8Json.Internal.NumberConverter.WriteSingle(ref Buffer, Offset, value);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDouble(double value) {
            Offset += Utf8Json.Internal.NumberConverter.WriteDouble(ref Buffer, Offset, value);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteByte(byte value) {
            Offset += Utf8Json.Internal.NumberConverter.WriteUInt64(ref Buffer, Offset, (ulong)value);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt16(ushort value) {
            Offset += Utf8Json.Internal.NumberConverter.WriteUInt64(ref Buffer, Offset, (ulong)value);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt32(uint value) {
            Offset += Utf8Json.Internal.NumberConverter.WriteUInt64(ref Buffer, Offset, (ulong)value);
        }

        public void WriteUInt64(ulong value) {
            Offset += Utf8Json.Internal.NumberConverter.WriteUInt64(ref Buffer, Offset, value);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSByte(sbyte value) {
            Offset += Utf8Json.Internal.NumberConverter.WriteInt64(ref Buffer, Offset, (long)value);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt16(short value) {
            Offset += Utf8Json.Internal.NumberConverter.WriteInt64(ref Buffer, Offset, (long)value);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt32(int value) {
            Offset += Utf8Json.Internal.NumberConverter.WriteInt64(ref Buffer, Offset, (long)value);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt64(long value) {
            Offset += Utf8Json.Internal.NumberConverter.WriteInt64(ref Buffer, Offset, value);
        }

        public void WriteText(string value) {
            if (value == null) {
                return;
            }
            var utf8 = (_UTF8 ??= new UTF8Encoding(false));

            var max = utf8.GetMaxByteCount(value.Length);
            BinaryUtil.EnsureCapacity(ref Buffer, Offset, max);
            Offset += utf8.GetBytes(value, 0, value.Length, Buffer, Offset);
        }

        public void WriteString(string? value) {
            if (string.IsNullOrEmpty(value)) {
                return;
            } else {
                var utf8 = (_UTF8 ??= new UTF8Encoding(false));

                BinaryUtil.EnsureCapacity(ref Buffer, Offset, value!.Length);

                // for JIT Optimization, for-loop i < str.Length
                for (int i = 0; i < value.Length; i++) {
                    var c = value[i];
                    if ((uint)c < 127) {
                        Buffer[Offset++] = (byte)c;
                        continue;
                    } else {
                        var max = utf8.GetMaxByteCount(value.Length - i);
                        BinaryUtil.EnsureCapacity(ref Buffer, Offset, max);

                        Offset += utf8.GetBytes(value, i, value.Length - i, Buffer, Offset);
                        return;
                    }
                }
            }
        }

        public void WriteQuotedString(string? value) {
            if (value == null) {
                WriteNull();
                return;
            } else {
                var utf8 = (_UTF8 ??= new UTF8Encoding(false));

                // single-path escape

                // nonescaped-ensure
                var startoffset = Offset;
                var max = utf8.GetMaxByteCount(value.Length) + 2;
                BinaryUtil.EnsureCapacity(ref Buffer, startoffset, max);

                var from = 0;
                var to = value.Length;

                Buffer[Offset++] = (byte)'\"';

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
                    BinaryUtil.EnsureCapacity(ref Buffer, startoffset, max); // check +escape capacity

                    Offset += utf8.GetBytes(value, from, i - from, Buffer, Offset);
                    from = i + 1;
                    Buffer[Offset++] = (byte)'\\';
                    Buffer[Offset++] = escapeChar;
                }

                if (from != value.Length) {
                    Offset += utf8.GetBytes(value, from, value.Length - from, Buffer, Offset);
                }

                Buffer[Offset++] = (byte)'\"';
            }
        }

        internal static class MemoryPool {
            [ThreadStatic]
            static byte[]? buffer = null;

            public static byte[] GetBuffer() {
                if (buffer == null) {
                    buffer = new byte[65536];
                }
                return buffer;
            }
        }
    }
}
