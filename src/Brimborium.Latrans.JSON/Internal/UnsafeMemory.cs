#if NETSTANDARD

using System.Runtime.CompilerServices;

namespace Brimborium.Latrans.JSON.Internal
{
    public static partial class UnsafeMemory32
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw4(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw5(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 1) = *(int*)(pSrc + 1);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw6(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 2) = *(int*)(pSrc + 2);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw7(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 3) = *(int*)(pSrc + 3);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw8(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw9(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 5) = *(int*)(pSrc + 5);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw10(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 6) = *(int*)(pSrc + 6);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw11(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 7) = *(int*)(pSrc + 7);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw12(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 8) = *(int*)(pSrc + 8);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw13(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 8) = *(int*)(pSrc + 8);
                *(int*)(pDst + 9) = *(int*)(pSrc + 9);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw14(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 8) = *(int*)(pSrc + 8);
                *(int*)(pDst + 10) = *(int*)(pSrc + 10);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw15(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 8) = *(int*)(pSrc + 8);
                *(int*)(pDst + 11) = *(int*)(pSrc + 11);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw16(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 8) = *(int*)(pSrc + 8);
                *(int*)(pDst + 12) = *(int*)(pSrc + 12);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw17(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 8) = *(int*)(pSrc + 8);
                *(int*)(pDst + 12) = *(int*)(pSrc + 12);
                *(int*)(pDst + 13) = *(int*)(pSrc + 13);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw18(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 8) = *(int*)(pSrc + 8);
                *(int*)(pDst + 12) = *(int*)(pSrc + 12);
                *(int*)(pDst + 14) = *(int*)(pSrc + 14);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw19(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 8) = *(int*)(pSrc + 8);
                *(int*)(pDst + 12) = *(int*)(pSrc + 12);
                *(int*)(pDst + 15) = *(int*)(pSrc + 15);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw20(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 8) = *(int*)(pSrc + 8);
                *(int*)(pDst + 12) = *(int*)(pSrc + 12);
                *(int*)(pDst + 16) = *(int*)(pSrc + 16);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw21(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 8) = *(int*)(pSrc + 8);
                *(int*)(pDst + 12) = *(int*)(pSrc + 12);
                *(int*)(pDst + 16) = *(int*)(pSrc + 16);
                *(int*)(pDst + 17) = *(int*)(pSrc + 17);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw22(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 8) = *(int*)(pSrc + 8);
                *(int*)(pDst + 12) = *(int*)(pSrc + 12);
                *(int*)(pDst + 16) = *(int*)(pSrc + 16);
                *(int*)(pDst + 18) = *(int*)(pSrc + 18);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw23(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 8) = *(int*)(pSrc + 8);
                *(int*)(pDst + 12) = *(int*)(pSrc + 12);
                *(int*)(pDst + 16) = *(int*)(pSrc + 16);
                *(int*)(pDst + 19) = *(int*)(pSrc + 19);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw24(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 8) = *(int*)(pSrc + 8);
                *(int*)(pDst + 12) = *(int*)(pSrc + 12);
                *(int*)(pDst + 16) = *(int*)(pSrc + 16);
                *(int*)(pDst + 20) = *(int*)(pSrc + 20);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw25(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 8) = *(int*)(pSrc + 8);
                *(int*)(pDst + 12) = *(int*)(pSrc + 12);
                *(int*)(pDst + 16) = *(int*)(pSrc + 16);
                *(int*)(pDst + 20) = *(int*)(pSrc + 20);
                *(int*)(pDst + 21) = *(int*)(pSrc + 21);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw26(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 8) = *(int*)(pSrc + 8);
                *(int*)(pDst + 12) = *(int*)(pSrc + 12);
                *(int*)(pDst + 16) = *(int*)(pSrc + 16);
                *(int*)(pDst + 20) = *(int*)(pSrc + 20);
                *(int*)(pDst + 22) = *(int*)(pSrc + 22);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw27(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 8) = *(int*)(pSrc + 8);
                *(int*)(pDst + 12) = *(int*)(pSrc + 12);
                *(int*)(pDst + 16) = *(int*)(pSrc + 16);
                *(int*)(pDst + 20) = *(int*)(pSrc + 20);
                *(int*)(pDst + 23) = *(int*)(pSrc + 23);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw28(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 8) = *(int*)(pSrc + 8);
                *(int*)(pDst + 12) = *(int*)(pSrc + 12);
                *(int*)(pDst + 16) = *(int*)(pSrc + 16);
                *(int*)(pDst + 20) = *(int*)(pSrc + 20);
                *(int*)(pDst + 24) = *(int*)(pSrc + 24);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw29(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 8) = *(int*)(pSrc + 8);
                *(int*)(pDst + 12) = *(int*)(pSrc + 12);
                *(int*)(pDst + 16) = *(int*)(pSrc + 16);
                *(int*)(pDst + 20) = *(int*)(pSrc + 20);
                *(int*)(pDst + 24) = *(int*)(pSrc + 24);
                *(int*)(pDst + 25) = *(int*)(pSrc + 25);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw30(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 8) = *(int*)(pSrc + 8);
                *(int*)(pDst + 12) = *(int*)(pSrc + 12);
                *(int*)(pDst + 16) = *(int*)(pSrc + 16);
                *(int*)(pDst + 20) = *(int*)(pSrc + 20);
                *(int*)(pDst + 24) = *(int*)(pSrc + 24);
                *(int*)(pDst + 26) = *(int*)(pSrc + 26);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw31(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(int*)(pDst + 0) = *(int*)(pSrc + 0);
                *(int*)(pDst + 4) = *(int*)(pSrc + 4);
                *(int*)(pDst + 8) = *(int*)(pSrc + 8);
                *(int*)(pDst + 12) = *(int*)(pSrc + 12);
                *(int*)(pDst + 16) = *(int*)(pSrc + 16);
                *(int*)(pDst + 20) = *(int*)(pSrc + 20);
                *(int*)(pDst + 24) = *(int*)(pSrc + 24);
                *(int*)(pDst + 27) = *(int*)(pSrc + 27);
            }

            writer.offset += src.Length;
        }

    }

    public static partial class UnsafeMemory64
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw8(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw9(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 1) = *(long*)(pSrc + 1);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw10(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 2) = *(long*)(pSrc + 2);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw11(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 3) = *(long*)(pSrc + 3);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw12(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 4) = *(long*)(pSrc + 4);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw13(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 5) = *(long*)(pSrc + 5);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw14(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 6) = *(long*)(pSrc + 6);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw15(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 7) = *(long*)(pSrc + 7);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw16(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 8) = *(long*)(pSrc + 8);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw17(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 8) = *(long*)(pSrc + 8);
                *(long*)(pDst + 9) = *(long*)(pSrc + 9);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw18(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 8) = *(long*)(pSrc + 8);
                *(long*)(pDst + 10) = *(long*)(pSrc + 10);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw19(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 8) = *(long*)(pSrc + 8);
                *(long*)(pDst + 11) = *(long*)(pSrc + 11);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw20(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 8) = *(long*)(pSrc + 8);
                *(long*)(pDst + 12) = *(long*)(pSrc + 12);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw21(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 8) = *(long*)(pSrc + 8);
                *(long*)(pDst + 13) = *(long*)(pSrc + 13);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw22(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 8) = *(long*)(pSrc + 8);
                *(long*)(pDst + 14) = *(long*)(pSrc + 14);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw23(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 8) = *(long*)(pSrc + 8);
                *(long*)(pDst + 15) = *(long*)(pSrc + 15);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw24(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 8) = *(long*)(pSrc + 8);
                *(long*)(pDst + 16) = *(long*)(pSrc + 16);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw25(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 8) = *(long*)(pSrc + 8);
                *(long*)(pDst + 16) = *(long*)(pSrc + 16);
                *(long*)(pDst + 17) = *(long*)(pSrc + 17);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw26(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 8) = *(long*)(pSrc + 8);
                *(long*)(pDst + 16) = *(long*)(pSrc + 16);
                *(long*)(pDst + 18) = *(long*)(pSrc + 18);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw27(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 8) = *(long*)(pSrc + 8);
                *(long*)(pDst + 16) = *(long*)(pSrc + 16);
                *(long*)(pDst + 19) = *(long*)(pSrc + 19);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw28(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 8) = *(long*)(pSrc + 8);
                *(long*)(pDst + 16) = *(long*)(pSrc + 16);
                *(long*)(pDst + 20) = *(long*)(pSrc + 20);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw29(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 8) = *(long*)(pSrc + 8);
                *(long*)(pDst + 16) = *(long*)(pSrc + 16);
                *(long*)(pDst + 21) = *(long*)(pSrc + 21);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw30(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 8) = *(long*)(pSrc + 8);
                *(long*)(pDst + 16) = *(long*)(pSrc + 16);
                *(long*)(pDst + 22) = *(long*)(pSrc + 22);
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw31(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(long*)(pDst + 0) = *(long*)(pSrc + 0);
                *(long*)(pDst + 8) = *(long*)(pSrc + 8);
                *(long*)(pDst + 16) = *(long*)(pSrc + 16);
                *(long*)(pDst + 23) = *(long*)(pSrc + 23);
            }

            writer.offset += src.Length;
        }

    }
}

#endif