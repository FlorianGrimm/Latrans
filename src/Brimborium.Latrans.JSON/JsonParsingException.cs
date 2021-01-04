#nullable disable

using System;

using Brimborium.Latrans.JSON.Internal;

namespace Brimborium.Latrans.JSON {
    public class JsonParsingException : Exception
    {
        WeakReference underyingBytes;
        int limit;
        public int Offset { get; private set; }
        public string ActualChar { get; set; }

        public JsonParsingException(string message)
            : base(message)
        {

        }

        public JsonParsingException(string message, byte[] underlyingBytes, int offset, int limit, string actualChar)
            : base(message)
        {
            this.underyingBytes = new WeakReference(underlyingBytes);
            this.Offset = offset;
            this.ActualChar = actualChar;
            this.limit = limit;
        }

        /// <summary>
        /// Underlying bytes is may be a pooling buffer, be careful to use it. If lost reference or can not handled byte[], return null.
        /// </summary>
        public byte[] GetUnderlyingByteArrayUnsafe()
        {
            return underyingBytes.Target as byte[];
        }

        /// <summary>
        /// Underlying bytes is may be a pooling buffer, be careful to use it. If lost reference or can not handled byte[], return null.
        /// </summary>
        public string GetUnderlyingStringUnsafe()
        {
            var bytes = underyingBytes.Target as byte[];
            if (bytes != null)
            {
                return StringEncoding.UTF8.GetString(bytes, 0, limit) + "...";
            }
            return null;
        }
    }
}
