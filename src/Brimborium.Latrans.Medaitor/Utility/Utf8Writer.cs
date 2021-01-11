using Brimborium.Latrans.Mediator;

using Microsoft.IO;

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;


namespace Brimborium.Latrans.Utility {
    public struct Utf8Writer {
        private static readonly RecyclableMemoryStreamManager manager = new RecyclableMemoryStreamManager();
        //System.Text.Json.Utf8JsonWriter
        private static Encoding? _UTF8;
        private readonly Stream _Stream;
        private byte[] _Buffer;
        private int _Offset;

        public Utf8Writer(Stream stream, byte[]? buffer = null) {
            this._Stream = stream;
            this._Buffer = buffer ?? MemoryPool.GetBuffer();
            this._Offset = 0;
        }

        public byte[] ToUtf8ByteArray() {
            if (this._Buffer == null) {
                return Array.Empty<byte>();
            } else {
                return BinaryUtil.FastCloneWithResize(this._Buffer, this._Offset);
            }
        }

        public override string ToString() {
            return string.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EnsureCapacity(int appendLength, bool ignoreIfTooSmall) {
            if ((this._Offset + appendLength) > this._Buffer.Length) {
                if (this._Offset > 0) {
                    this._Stream.Write(this._Buffer, 0, this._Offset);
                    this._Offset = 0;
                }
                if ((this._Offset + appendLength) < this._Buffer.Length) {
                    return true;
                } else {
                    if (ignoreIfTooSmall) {
                        return false;
                    } else {
                        throw new InvalidOperationException($"appendLength :{appendLength} is too big.");
                    }
                }
            } else {
                return true;
            }
        }

        public void Flush() {
            if (this._Offset > 0) {
                this._Stream.Write(this._Buffer, 0, this._Offset);
                this._Offset = 0;
            }
        }

        public void WriteRaw(byte rawValue) {
            this.EnsureCapacity(1, false);
            this._Buffer[this._Offset++] = rawValue;
        }

        public void WriteRaw(byte[] rawValue) {
            if (this.EnsureCapacity(rawValue.Length, true)) {
                System.Buffer.BlockCopy(rawValue, 0, this._Buffer, this._Offset, rawValue.Length);
                this._Offset += rawValue.Length;
            } else {
                this._Stream.Write(rawValue, 0, rawValue.Length);
            }
        }

        public void WriteNull() {
            this.EnsureCapacity(4, false);
            this._Buffer[this._Offset + 0] = (byte)'n';
            this._Buffer[this._Offset + 1] = (byte)'u';
            this._Buffer[this._Offset + 2] = (byte)'l';
            this._Buffer[this._Offset + 3] = (byte)'l';
            this._Offset += 4;
        }

        public void WriteBoolean(bool value) {
            if (value) {
                this.EnsureCapacity(4, false);
                this._Buffer[this._Offset + 0] = (byte)'t';
                this._Buffer[this._Offset + 1] = (byte)'r';
                this._Buffer[this._Offset + 2] = (byte)'u';
                this._Buffer[this._Offset + 3] = (byte)'e';
                this._Offset += 4;
            } else {
                this.EnsureCapacity(5, false);
                this._Buffer[this._Offset + 0] = (byte)'f';
                this._Buffer[this._Offset + 1] = (byte)'a';
                this._Buffer[this._Offset + 2] = (byte)'l';
                this._Buffer[this._Offset + 3] = (byte)'s';
                this._Buffer[this._Offset + 4] = (byte)'e';
                this._Offset += 5;
            }
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public void WriteSingle(Single value) {
        //    Offset += Utf8Json.Internal.NumberConverter.WriteSingle(ref Buffer, Offset, value);
        //}

        ////[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public void WriteDouble(double value) {
        //    Offset += Utf8Json.Internal.NumberConverter.WriteDouble(ref Buffer, Offset, value);
        //}

        ////[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public void WriteByte(byte value) {
        //    Offset += Utf8Json.Internal.NumberConverter.WriteUInt64(ref Buffer, Offset, (ulong)value);
        //}

        ////[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public void WriteUInt16(ushort value) {
        //    Offset += Utf8Json.Internal.NumberConverter.WriteUInt64(ref Buffer, Offset, (ulong)value);
        //}

        ////[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public void WriteUInt32(uint value) {
        //    Offset += Utf8Json.Internal.NumberConverter.WriteUInt64(ref Buffer, Offset, (ulong)value);
        //}

        //public void WriteUInt64(ulong value) {
        //    Offset += Utf8Json.Internal.NumberConverter.WriteUInt64(ref Buffer, Offset, value);
        //}

        ////[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public void WriteSByte(sbyte value) {
        //    Offset += Utf8Json.Internal.NumberConverter.WriteInt64(ref Buffer, Offset, (long)value);
        //}

        ////[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public void WriteInt16(short value) {
        //    Offset += Utf8Json.Internal.NumberConverter.WriteInt64(ref Buffer, Offset, (long)value);
        //}

        ////[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public void WriteInt32(int value) {
        //    Offset += Utf8Json.Internal.NumberConverter.WriteInt64(ref Buffer, Offset, (long)value);
        //}

        ////[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public void WriteInt64(long value) {
        //    Offset += Utf8Json.Internal.NumberConverter.WriteInt64(ref Buffer, Offset, value);
        //}

        public void WriteBigString(string value) {
            if (value == null) {
                return;
            }
            var utf8 = (_UTF8 ??= new UTF8Encoding(false));

            var max = (value.Length < 1024) ? utf8.GetMaxByteCount(value.Length) : utf8.GetByteCount(value);
            if (this.EnsureCapacity(max, true)) {
                this._Offset += utf8.GetBytes(value, 0, value.Length, this._Buffer, this._Offset);
            } else {
                var bytes = utf8.GetBytes(value);
                this._Stream.Write(bytes, 0, bytes.Length);
            }

        }

        public void WriteString(string? value) {
            if (value is null || string.IsNullOrEmpty(value)) {
                return;
            } else {
                var utf8 = (_UTF8 ??= new UTF8Encoding(false));
                var max = utf8.GetMaxByteCount(value.Length);
                if (this.EnsureCapacity(max, true)) {
                    // for JIT Optimization, for-loop i < str.Length
                    for (int i = 0; i < value.Length; i++) {
                        var c = value[i];
                        if ((uint)c < 127) {
                            this._Buffer[this._Offset++] = (byte)c;
                            continue;
                        } else {
                            this._Offset += utf8.GetBytes(value, i, value.Length - i, this._Buffer, this._Offset);
                            return;
                        }
                    }
                } else {
                    WriteBigString(value);
                }
            }
        }

        public void WriteQuotedString(string? value) {
            if (value == null) {
                this.WriteNull();
                return;
            } else {
                var utf8 = (_UTF8 ??= new UTF8Encoding(false));

                // single-path escape

                // nonescaped-ensure
                var startoffset = this._Offset;
                var max = utf8.GetMaxByteCount(value.Length) + 2;
                BinaryUtil.EnsureCapacity(ref this._Buffer, startoffset, max);

                var from = 0;
                var to = value.Length;

                this._Buffer[this._Offset++] = (byte)'\"';

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
                    BinaryUtil.EnsureCapacity(ref this._Buffer, startoffset, max); // check +escape capacity

                    this._Offset += utf8.GetBytes(value, from, i - from, this._Buffer, this._Offset);
                    from = i + 1;
                    this._Buffer[this._Offset++] = (byte)'\\';
                    this._Buffer[this._Offset++] = escapeChar;
                }

                if (from != value.Length) {
                    this._Offset += utf8.GetBytes(value, from, value.Length - from, this._Buffer, this._Offset);
                }

                this._Buffer[this._Offset++] = (byte)'\"';
            }
        }

        internal static class MemoryPool {
            [ThreadStatic]
            static byte[]? _Buffer = null;

            public static byte[] GetBuffer() {
                if (_Buffer == null) {
                    _Buffer = new byte[65536];
                }
                return _Buffer;
            }
        }
    }
}