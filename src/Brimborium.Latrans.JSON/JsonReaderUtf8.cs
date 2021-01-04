#nullable disable

using System;
using System.Text;
using Brimborium.Latrans.JSON.Internal;

using System.Runtime.CompilerServices;

namespace Brimborium.Latrans.JSON {
    public class JsonReaderUtf8 : JsonReader {
        private static readonly ArraySegment<byte> nullTokenSegment = new ArraySegment<byte>(new byte[] { 110, 117, 108, 108 }, 0, 4);
        private static readonly byte[] bom = Encoding.UTF8.GetPreamble();

        private readonly byte[] bytes;
        private int offset;

        public JsonReaderUtf8(byte[] bytes)
            : this(bytes, 0) {

        }

        public JsonReaderUtf8(byte[] bytes, int offset) {
            this.bytes = bytes;
            this.offset = offset;

            // skip bom
            if (bytes.Length >= 3) {
                if (bytes[offset] == bom[0] && bytes[offset + 1] == bom[1] && bytes[offset + 2] == bom[2]) {
                    this.offset = offset += 3;
                }
            }
        }

        private JsonParsingException CreateParsingException(string expected) {
            var actual = ((char)this.bytes[this.offset]).ToString();
            var pos = this.offset;

            try {
                var token = this.GetCurrentJsonToken();
                switch (token) {
                    case JsonToken.Number:
                        var ns = this.ReadNumberSegment();
                        actual = StringEncoding.UTF8.GetString(ns.Array, ns.Offset, ns.Count);
                        break;
                    case JsonToken.String:
                        actual = "\"" + this.ReadString() + "\"";
                        break;
                    case JsonToken.True:
                        actual = "true";
                        break;
                    case JsonToken.False:
                        actual = "false";
                        break;
                    case JsonToken.Null:
                        actual = "null";
                        break;
                    default:
                        break;
                }
            } catch { }

            return new JsonParsingException("expected:'" + expected + "', actual:'" + actual + "', at offset:" + pos, this.bytes, pos, this.offset, actual);
        }

        private JsonParsingException CreateParsingExceptionMessage(string message) {
            var actual = ((char)this.bytes[this.offset]).ToString();
            var pos = this.offset;

            return new JsonParsingException(message, this.bytes, pos, pos, actual);
        }

        private bool IsInRange {
            get {
                return this.offset < this.bytes.Length;
            }
        }

        public override void AdvanceOffset(int offset) {
            this.offset += offset;
        }

        public override byte[] GetBufferUnsafe() {
            return this.bytes;
        }

        public override int GetCurrentOffsetUnsafe() {
            return this.offset;
        }

        public override JsonToken GetCurrentJsonToken() {
            this.SkipWhiteSpace();
            if (this.offset < this.bytes.Length) {
                var c = this.bytes[this.offset];
                switch (c) {
                    case (byte)'{': return JsonToken.BeginObject;
                    case (byte)'}': return JsonToken.EndObject;
                    case (byte)'[': return JsonToken.BeginArray;
                    case (byte)']': return JsonToken.EndArray;
                    case (byte)'t': return JsonToken.True;
                    case (byte)'f': return JsonToken.False;
                    case (byte)'n': return JsonToken.Null;
                    case (byte)',': return JsonToken.ValueSeparator;
                    case (byte)':': return JsonToken.NameSeparator;
                    case (byte)'-': return JsonToken.Number;
                    case (byte)'0': return JsonToken.Number;
                    case (byte)'1': return JsonToken.Number;
                    case (byte)'2': return JsonToken.Number;
                    case (byte)'3': return JsonToken.Number;
                    case (byte)'4': return JsonToken.Number;
                    case (byte)'5': return JsonToken.Number;
                    case (byte)'6': return JsonToken.Number;
                    case (byte)'7': return JsonToken.Number;
                    case (byte)'8': return JsonToken.Number;
                    case (byte)'9': return JsonToken.Number;
                    case (byte)'\"': return JsonToken.String;
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 28:
                    case 29:
                    case 30:
                    case 31:
                    case 32:
                    case 33:
                    case 35:
                    case 36:
                    case 37:
                    case 38:
                    case 39:
                    case 40:
                    case 41:
                    case 42:
                    case 43:
                    case 46:
                    case 47:
                    case 59:
                    case 60:
                    case 61:
                    case 62:
                    case 63:
                    case 64:
                    case 65:
                    case 66:
                    case 67:
                    case 68:
                    case 69:
                    case 70:
                    case 71:
                    case 72:
                    case 73:
                    case 74:
                    case 75:
                    case 76:
                    case 77:
                    case 78:
                    case 79:
                    case 80:
                    case 81:
                    case 82:
                    case 83:
                    case 84:
                    case 85:
                    case 86:
                    case 87:
                    case 88:
                    case 89:
                    case 90:
                    case 92:
                    case 94:
                    case 95:
                    case 96:
                    case 97:
                    case 98:
                    case 99:
                    case 100:
                    case 101:
                    case 103:
                    case 104:
                    case 105:
                    case 106:
                    case 107:
                    case 108:
                    case 109:
                    case 111:
                    case 112:
                    case 113:
                    case 114:
                    case 115:
                    case 117:
                    case 118:
                    case 119:
                    case 120:
                    case 121:
                    case 122:
                    default:
                        return JsonToken.None;
                }
            } else {
                return JsonToken.None;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SkipWhiteSpace() {
            // eliminate array bound check
            for (int i = this.offset; i < this.bytes.Length; i++) {
                switch (this.bytes[i]) {
                    case 0x20: // Space
                    case 0x09: // Horizontal tab
                    case 0x0A: // Line feed or New line
                    case 0x0D: // Carriage return
                        continue;
                    case (byte)'/': // BeginComment
                        i = ReadComment(this.bytes, i);
                        continue;
                    // optimize skip jumptable
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 11:
                    case 12:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 28:
                    case 29:
                    case 30:
                    case 31:
                    case 33:
                    case 34:
                    case 35:
                    case 36:
                    case 37:
                    case 38:
                    case 39:
                    case 40:
                    case 41:
                    case 42:
                    case 43:
                    case 44:
                    case 45:
                    case 46:
                    default:
                        this.offset = i;
                        return; // end
                }
            }

            this.offset = this.bytes.Length;
        }

        public override bool ReadIsNull() {
            this.SkipWhiteSpace();
            if (this.IsInRange && this.bytes[this.offset] == 'n') {
                if (this.bytes[this.offset + 1] != 'u') {
                    goto ERROR;
                }

                if (this.bytes[this.offset + 2] != 'l') {
                    goto ERROR;
                }

                if (this.bytes[this.offset + 3] != 'l') {
                    goto ERROR;
                }

                this.offset += 4;
                return true;
            } else {
                return false;
            }

        ERROR:
            throw this.CreateParsingException("null");
        }

        public override bool ReadIsBeginArray() {
            this.SkipWhiteSpace();
            if (this.IsInRange && this.bytes[this.offset] == '[') {
                this.offset += 1;
                return true;
            } else {
                return false;
            }
        }

        public override void ReadIsBeginArrayWithVerify() {
            if (!this.ReadIsBeginArray()) {
                throw this.CreateParsingException("[");
            }
        }

        public override bool ReadIsEndArray() {
            this.SkipWhiteSpace();
            if (this.IsInRange && this.bytes[this.offset] == ']') {
                this.offset += 1;
                return true;
            } else {
                return false;
            }
        }

        public override void ReadIsEndArrayWithVerify() {
            if (!this.ReadIsEndArray()) {
                throw this.CreateParsingException("]");
            }
        }

        public override bool ReadIsEndArrayWithSkipValueSeparator(ref int count) {
            this.SkipWhiteSpace();
            if (this.IsInRange && this.bytes[this.offset] == ']') {
                this.offset += 1;
                return true;
            } else {
                if (count++ != 0) {
                    this.ReadIsValueSeparatorWithVerify();
                }
                return false;
            }
        }

        /// <summary>
        /// Convinient pattern of ReadIsBeginArrayWithVerify + while(!ReadIsEndArrayWithSkipValueSeparator)
        /// </summary>
        public override bool ReadIsInArray(ref int count) {
            if (count == 0) {
                this.ReadIsBeginArrayWithVerify();
                if (this.ReadIsEndArray()) {
                    return false;
                }
            } else {
                if (this.ReadIsEndArray()) {
                    return false;
                } else {
                    this.ReadIsValueSeparatorWithVerify();
                }
            }

            count++;
            return true;
        }

        public override bool ReadIsBeginObject() {
            this.SkipWhiteSpace();
            if (this.IsInRange && this.bytes[this.offset] == '{') {
                this.offset += 1;
                return true;
            } else {
                return false;
            }
        }

        public override void ReadIsBeginObjectWithVerify() {
            if (!this.ReadIsBeginObject()) {
                throw this.CreateParsingException("{");
            }
        }

        public override bool ReadIsEndObject() {
            this.SkipWhiteSpace();
            if (this.IsInRange && this.bytes[this.offset] == '}') {
                this.offset += 1;
                return true;
            } else {
                return false;
            }
        }

        public override void ReadIsEndObjectWithVerify() {
            if (!this.ReadIsEndObject()) {
                throw this.CreateParsingException("}");
            }
        }

        public override bool ReadIsEndObjectWithSkipValueSeparator(ref int count) {
            this.SkipWhiteSpace();
            if (this.IsInRange && this.bytes[this.offset] == '}') {
                this.offset += 1;
                return true;
            } else {
                if (count++ != 0) {
                    this.ReadIsValueSeparatorWithVerify();
                }
                return false;
            }
        }

        /// <summary>
        /// Convinient pattern of ReadIsBeginObjectWithVerify + while(!ReadIsEndObjectWithSkipValueSeparator)
        /// </summary>
        public override bool ReadIsInObject(ref int count) {
            if (count == 0) {
                this.ReadIsBeginObjectWithVerify();
                if (this.ReadIsEndObject()) {
                    return false;
                }
            } else {
                if (this.ReadIsEndObject()) {
                    return false;
                } else {
                    this.ReadIsValueSeparatorWithVerify();
                }
            }

            count++;
            return true;
        }

        public override bool ReadIsValueSeparator() {
            this.SkipWhiteSpace();
            if (this.IsInRange && this.bytes[this.offset] == ',') {
                this.offset += 1;
                return true;
            } else {
                return false;
            }
        }

        public override void ReadIsValueSeparatorWithVerify() {
            if (!this.ReadIsValueSeparator()) {
                throw this.CreateParsingException(",");
            }
        }

        public override bool ReadIsNameSeparator() {
            this.SkipWhiteSpace();
            if (this.IsInRange && this.bytes[this.offset] == ':') {
                this.offset += 1;
                return true;
            } else {
                return false;
            }
        }

        public override void ReadIsNameSeparatorWithVerify() {
            if (!this.ReadIsNameSeparator()) {
                throw this.CreateParsingException(":");
            }
        }

        private void ReadStringSegmentCore(out byte[] resultBytes, out int resultOffset, out int resultLength) {
            // SkipWhiteSpace is already called from IsNull

            byte[] builder = null;
            var builderOffset = 0;
            char[] codePointStringBuffer = null;
            var codePointStringOffet = 0;

            if (this.bytes[this.offset] != '\"') {
                throw this.CreateParsingException("String Begin Token");
            }

            this.offset++;

            var from = this.offset;

            // eliminate array-bound check
            for (int i = this.offset; i < this.bytes.Length; i++) {
                byte escapeCharacter = 0;
                switch (this.bytes[i]) {
                    case (byte)'\\': // escape character
                        switch ((char)this.bytes[i + 1]) {
                            case '"':
                            case '\\':
                            case '/':
                                escapeCharacter = this.bytes[i + 1];
                                goto COPY;
                            case 'b':
                                escapeCharacter = (byte)'\b';
                                goto COPY;
                            case 'f':
                                escapeCharacter = (byte)'\f';
                                goto COPY;
                            case 'n':
                                escapeCharacter = (byte)'\n';
                                goto COPY;
                            case 'r':
                                escapeCharacter = (byte)'\r';
                                goto COPY;
                            case 't':
                                escapeCharacter = (byte)'\t';
                                goto COPY;
                            case 'u':
                                if (codePointStringBuffer == null) {
                                    codePointStringBuffer = StringBuilderCache.GetCodePointStringBuffer();
                                }

                                if (codePointStringOffet == 0) {
                                    if (builder == null) {
                                        builder = StringBuilderCache.GetBuffer();
                                    }

                                    var copyCount = i - from;
                                    ByteArrayUtil.EnsureCapacity(ref builder, builderOffset, copyCount + 1); // require + 1
                                    Buffer.BlockCopy(this.bytes, from, builder, builderOffset, copyCount);
                                    builderOffset += copyCount;
                                }

                                if (codePointStringBuffer.Length == codePointStringOffet) {
                                    Array.Resize(ref codePointStringBuffer, codePointStringBuffer.Length * 2);
                                }

                                var a = (char)this.bytes[i + 2];
                                var b = (char)this.bytes[i + 3];
                                var c = (char)this.bytes[i + 4];
                                var d = (char)this.bytes[i + 5];
                                var codepoint = GetCodePoint(a, b, c, d);
                                codePointStringBuffer[codePointStringOffet++] = (char)codepoint;
                                i += 5;
                                this.offset += 6;
                                from = this.offset;
                                continue;
                            default:
                                throw this.CreateParsingExceptionMessage("Bad JSON escape.");
                        }
                    case (byte)'"': // endtoken
                        this.offset++;
                        goto END;
                    default: // string
                        if (codePointStringOffet != 0) {
                            if (builder == null) {
                                builder = StringBuilderCache.GetBuffer();
                            }

                            ByteArrayUtil.EnsureCapacity(ref builder, builderOffset, StringEncoding.UTF8.GetMaxByteCount(codePointStringOffet));
                            builderOffset += StringEncoding.UTF8.GetBytes(codePointStringBuffer, 0, codePointStringOffet, builder, builderOffset);
                            codePointStringOffet = 0;
                        }
                        this.offset++;
                        continue;
                }

            COPY:
                {
                    if (builder == null) {
                        builder = StringBuilderCache.GetBuffer();
                    }

                    if (codePointStringOffet != 0) {
                        ByteArrayUtil.EnsureCapacity(ref builder, builderOffset, StringEncoding.UTF8.GetMaxByteCount(codePointStringOffet));
                        builderOffset += StringEncoding.UTF8.GetBytes(codePointStringBuffer, 0, codePointStringOffet, builder, builderOffset);
                        codePointStringOffet = 0;
                    }

                    var copyCount = i - from;
                    ByteArrayUtil.EnsureCapacity(ref builder, builderOffset, copyCount + 1); // require + 1!
                    Buffer.BlockCopy(this.bytes, from, builder, builderOffset, copyCount);
                    builderOffset += copyCount;
                    builder[builderOffset++] = escapeCharacter;
                    i += 1;
                    this.offset += 2;
                    from = this.offset;
                }
            }

            resultLength = 0;
            resultBytes = null;
            resultOffset = 0;
            throw this.CreateParsingException("String End Token");

        END:
            if (builderOffset == 0 && codePointStringOffet == 0) // no escape
            {
                resultBytes = this.bytes;
                resultOffset = from;
                resultLength = this.offset - 1 - from; // skip last quote
            } else {
                if (builder == null) {
                    builder = StringBuilderCache.GetBuffer();
                }

                if (codePointStringOffet != 0) {
                    ByteArrayUtil.EnsureCapacity(ref builder, builderOffset, StringEncoding.UTF8.GetMaxByteCount(codePointStringOffet));
                    builderOffset += StringEncoding.UTF8.GetBytes(codePointStringBuffer, 0, codePointStringOffet, builder, builderOffset);
                    codePointStringOffet = 0;
                }

                var copyCount = this.offset - from - 1;
                ByteArrayUtil.EnsureCapacity(ref builder, builderOffset, copyCount);
                Buffer.BlockCopy(this.bytes, from, builder, builderOffset, copyCount);
                builderOffset += copyCount;

                resultBytes = builder;
                resultOffset = 0;
                resultLength = builderOffset;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetCodePoint(char a, char b, char c, char d) {
            return (((((ToNumber(a) * 16) + ToNumber(b)) * 16) + ToNumber(c)) * 16) + ToNumber(d);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ToNumber(char x) {
            if ('0' <= x && x <= '9') {
                return x - '0';
            } else if ('a' <= x && x <= 'f') {
                return x - 'a' + 10;
            } else if ('A' <= x && x <= 'F') {
                return x - 'A' + 10;
            }
            throw new JsonParsingException("Invalid Character" + x);
        }

        public override ArraySegment<byte> ReadStringSegmentUnsafe() {
            if (this.ReadIsNull()) {
                return nullTokenSegment;
            }

            byte[] bytes;
            int offset;
            int length;
            this.ReadStringSegmentCore(out bytes, out offset, out length);
            return new ArraySegment<byte>(bytes, offset, length);
        }

        public override string ReadString() {
            if (this.ReadIsNull()) {
                return null;
            }

            byte[] bytes;
            int offset;
            int length;
            this.ReadStringSegmentCore(out bytes, out offset, out length);

            return Encoding.UTF8.GetString(bytes, offset, length);
        }

        /// <summary>ReadString + ReadIsNameSeparatorWithVerify</summary>
        public override string ReadPropertyName() {
            var key = this.ReadString();
            this.ReadIsNameSeparatorWithVerify();
            return key;
        }

        /// <summary>Get raw string-span(do not unescape)</summary>
        public override ArraySegment<byte> ReadStringSegmentRaw() {
            ArraySegment<byte> key = default(ArraySegment<byte>);
            if (this.ReadIsNull()) {
                key = nullTokenSegment;
            } else {
                // SkipWhiteSpace is already called from IsNull
                if (this.bytes[this.offset++] != '\"') {
                    throw this.CreateParsingException("\"");
                }

                var from = this.offset;

                for (int i = this.offset; i < this.bytes.Length; i++) {
                    if (this.bytes[i] == (char)'\"') {
                        // is escape?
                        if (this.bytes[i - 1] == (char)'\\') {
                            continue;
                        } else {
                            this.offset = i + 1;
                            goto OK;
                        }
                    }
                }
                throw this.CreateParsingExceptionMessage("not found end string.");

            OK:
                key = new ArraySegment<byte>(this.bytes, from, this.offset - from - 1); // remove \"
            }

            return key;
        }

        /// <summary>Get raw string-span(do not unescape) + ReadIsNameSeparatorWithVerify</summary>
        public override ArraySegment<byte> ReadPropertyNameSegmentRaw() {
            var key = this.ReadStringSegmentRaw();
            this.ReadIsNameSeparatorWithVerify();
            return key;
        }

        public override bool ReadBoolean() {
            this.SkipWhiteSpace();
            if (this.bytes[this.offset] == 't') {
                if (this.bytes[this.offset + 1] != 'r') {
                    goto ERROR_TRUE;
                }

                if (this.bytes[this.offset + 2] != 'u') {
                    goto ERROR_TRUE;
                }

                if (this.bytes[this.offset + 3] != 'e') {
                    goto ERROR_TRUE;
                }

                this.offset += 4;
                return true;
            } else if (this.bytes[this.offset] == 'f') {
                if (this.bytes[this.offset + 1] != 'a') {
                    goto ERROR_FALSE;
                }

                if (this.bytes[this.offset + 2] != 'l') {
                    goto ERROR_FALSE;
                }

                if (this.bytes[this.offset + 3] != 's') {
                    goto ERROR_FALSE;
                }

                if (this.bytes[this.offset + 4] != 'e') {
                    goto ERROR_FALSE;
                }

                this.offset += 5;
                return false;
            } else {
                throw this.CreateParsingException("true | false");
            }

        ERROR_TRUE:
            throw this.CreateParsingException("true");
        ERROR_FALSE:
            throw this.CreateParsingException("false");
        }

        private static bool IsWordBreak(byte c) {
            switch (c) {
                case (byte)' ':
                case (byte)'{':
                case (byte)'}':
                case (byte)'[':
                case (byte)']':
                case (byte)',':
                case (byte)':':
                case (byte)'\"':
                    return true;
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                case 33:
                case 35:
                case 36:
                case 37:
                case 38:
                case 39:
                case 40:
                case 41:
                case 42:
                case 43:
                case 45:
                case 46:
                case 47:
                case 48:
                case 49:
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                case 56:
                case 57:
                case 59:
                case 60:
                case 61:
                case 62:
                case 63:
                case 64:
                case 65:
                case 66:
                case 67:
                case 68:
                case 69:
                case 70:
                case 71:
                case 72:
                case 73:
                case 74:
                case 75:
                case 76:
                case 77:
                case 78:
                case 79:
                case 80:
                case 81:
                case 82:
                case 83:
                case 84:
                case 85:
                case 86:
                case 87:
                case 88:
                case 89:
                case 90:
                case 92:
                case 94:
                case 95:
                case 96:
                case 97:
                case 98:
                case 99:
                case 100:
                case 101:
                case 102:
                case 103:
                case 104:
                case 105:
                case 106:
                case 107:
                case 108:
                case 109:
                case 110:
                case 111:
                case 112:
                case 113:
                case 114:
                case 115:
                case 116:
                case 117:
                case 118:
                case 119:
                case 120:
                case 121:
                case 122:
                default:
                    return false;
            }
        }

        public override void ReadNext() {
            var token = this.GetCurrentJsonToken();
            this.ReadNextCore(token);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReadNextCore(JsonToken token) {
            switch (token) {
                case JsonToken.BeginObject:
                case JsonToken.BeginArray:
                case JsonToken.ValueSeparator:
                case JsonToken.NameSeparator:
                case JsonToken.EndObject:
                case JsonToken.EndArray:
                    this.offset += 1;
                    break;
                case JsonToken.True:
                case JsonToken.Null:
                    this.offset += 4;
                    break;
                case JsonToken.False:
                    this.offset += 5;
                    break;
                case JsonToken.String:
                    this.offset += 1; // position is "\"";
                    for (int i = this.offset; i < this.bytes.Length; i++) {
                        if (this.bytes[i] == (char)'\"') {
                            // is escape?
                            if (this.bytes[i - 1] == (char)'\\') {
                                continue;
                            } else {
                                this.offset = i + 1;
                                return; // end
                            }
                        }
                    }
                    throw this.CreateParsingExceptionMessage("not found end string.");
                case JsonToken.Number:
                    for (int i = this.offset; i < this.bytes.Length; i++) {
                        if (IsWordBreak(this.bytes[i])) {
                            this.offset = i;
                            return;
                        }
                    }
                    this.offset = this.bytes.Length;
                    break;
                case JsonToken.None:
                default:
                    break;
            }
        }

        public override void ReadNextBlock() {
            var stack = 0;

        AGAIN:
            var token = this.GetCurrentJsonToken();
            switch (token) {
                case JsonToken.BeginObject:
                case JsonToken.BeginArray:
                    this.offset++;
                    stack++;
                    goto AGAIN;
                case JsonToken.EndObject:
                case JsonToken.EndArray:
                    this.offset++;
                    stack--;
                    if (stack != 0) {
                        goto AGAIN;
                    }
                    break;
                case JsonToken.True:
                case JsonToken.False:
                case JsonToken.Null:
                case JsonToken.String:
                case JsonToken.Number:
                case JsonToken.NameSeparator:
                case JsonToken.ValueSeparator:
                    do {
                        this.ReadNextCore(token);
                        token = this.GetCurrentJsonToken();
                    } while (stack != 0 && !((int)token < 5)); // !(None, Begin/EndObject, Begin/EndArray)

                    if (stack != 0) {
                        goto AGAIN;
                    }
                    break;
                case JsonToken.None:
                default:
                    break;
            }
        }

#if weichei
        public ArraySegment<byte> ReadNextBlockSegment() {
            var startOffset = this.offset;
            this.ReadNextBlock();
            return new ArraySegment<byte>(this.bytes, startOffset, this.offset - startOffset);
        }
#endif

        public override sbyte ReadSByte() {
            return checked((sbyte)this.ReadInt64());
        }

        public override short ReadInt16() {
            return checked((short)this.ReadInt64());
        }

        public override int ReadInt32() {
            return checked((int)this.ReadInt64());
        }

        public override long ReadInt64() {
            this.SkipWhiteSpace();

            int readCount;
            var v = NumberConverter.ReadInt64(this.bytes, this.offset, out readCount);
            if (readCount == 0) {
                throw this.CreateParsingException("Number Token");
            }

            this.offset += readCount;
            return v;
        }

        public override byte ReadByte() {
            return checked((byte)this.ReadUInt64());
        }

        public override ushort ReadUInt16() {
            return checked((ushort)this.ReadUInt64());
        }

        public override uint ReadUInt32() {
            return checked((uint)this.ReadUInt64());
        }

        public override ulong ReadUInt64() {
            this.SkipWhiteSpace();

            int readCount;
            var v = NumberConverter.ReadUInt64(this.bytes, this.offset, out readCount);
            if (readCount == 0) {
                throw this.CreateParsingException("Number Token");
            }
            this.offset += readCount;
            return v;
        }

        public override Single ReadSingle() {
            this.SkipWhiteSpace();
            int readCount;
            var v = Brimborium.Latrans.JSON.Internal.DoubleConversion.StringToDoubleConverter.ToSingle(this.bytes, this.offset, out readCount);
            if (readCount == 0) {
                throw this.CreateParsingException("Number Token");
            }
            this.offset += readCount;
            return v;
        }

        public override Double ReadDouble() {
            this.SkipWhiteSpace();
            int readCount;
            var v = Brimborium.Latrans.JSON.Internal.DoubleConversion.StringToDoubleConverter.ToDouble(this.bytes, this.offset, out readCount);
            if (readCount == 0) {
                throw this.CreateParsingException("Number Token");
            }
            this.offset += readCount;
            return v;
        }

        public ArraySegment<byte> ReadNumberSegment() {
            this.SkipWhiteSpace();
            var initialOffset = this.offset;
            for (int i = this.offset; i < this.bytes.Length; i++) {
                if (!NumberConverter.IsNumberRepresentation(this.bytes[i])) {
                    this.offset = i;
                    goto END;
                }
            }
            this.offset = this.bytes.Length;

        END:
            return new ArraySegment<byte>(this.bytes, initialOffset, this.offset - initialOffset);
        }

        // return last offset.
        private static int ReadComment(byte[] bytes, int offset) {
            // current token is '/'
            if (bytes[offset + 1] == '/') {
                // single line
                offset += 2;
                for (int i = offset; i < bytes.Length; i++) {
                    if (bytes[i] == '\r' || bytes[i] == '\n') {
                        return i;
                    }
                }

                throw new JsonParsingException("Can not find end token of single line comment(\r or \n).");
            } else if (bytes[offset + 1] == '*') {

                offset += 2; // '/' + '*';
                for (int i = offset; i < bytes.Length; i++) {
                    if (bytes[i] == '*' && bytes[i + 1] == '/') {
                        return i + 1;
                    }
                }
                throw new JsonParsingException("Can not find end token of multi line comment(*/).");
            }

            return offset;
        }

        internal static class StringBuilderCache {
            [ThreadStatic]
            private static byte[] buffer;

            [ThreadStatic]
            private static char[] codePointStringBuffer;

            public static byte[] GetBuffer() {
                if (buffer == null) {
                    buffer = new byte[65535];
                }
                return buffer;
            }

            public static char[] GetCodePointStringBuffer() {
                if (codePointStringBuffer == null) {
                    codePointStringBuffer = new char[65535];
                }
                return codePointStringBuffer;
            }
        }
    }
}
