#if NETSTANDARD

using System;
using System.Runtime.CompilerServices;

namespace Brimborium.Latrans.JSON.Internal
{
    // for string key property name write optimization.
    public static partial class UnsafeMemory {
        public static unsafe void MemoryCopy(JsonWriter writer, byte[] src) {
            if (writer is JsonWriterUtf8 jsonWriterUtf8) {
                MemoryCopyUtf8(jsonWriterUtf8, src);
            }
        }
    }

            
    public static partial class UnsafeMemory
    {
        public static readonly bool Is32Bit = (IntPtr.Size == 4);

        public static void WriteRaw(JsonWriterUtf8 writer, byte[] src)
        {
            switch (src.Length)
            {
                case 0: break;
                case 1: if (Is32Bit) { UnsafeMemory32.WriteRaw1(writer, src); } else { UnsafeMemory64.WriteRaw1(writer, src); } break;
                case 2: if (Is32Bit) { UnsafeMemory32.WriteRaw2(writer, src); } else { UnsafeMemory64.WriteRaw2(writer, src); } break;
                case 3: if (Is32Bit) { UnsafeMemory32.WriteRaw3(writer, src); } else { UnsafeMemory64.WriteRaw3(writer, src); } break;
                case 4: if (Is32Bit) { UnsafeMemory32.WriteRaw4(writer, src); } else { UnsafeMemory64.WriteRaw4(writer, src); } break;
                case 5: if (Is32Bit) { UnsafeMemory32.WriteRaw5(writer, src); } else { UnsafeMemory64.WriteRaw5(writer, src); } break;
                case 6: if (Is32Bit) { UnsafeMemory32.WriteRaw6(writer, src); } else { UnsafeMemory64.WriteRaw6(writer, src); } break;
                case 7: if (Is32Bit) { UnsafeMemory32.WriteRaw7(writer, src); } else { UnsafeMemory64.WriteRaw7(writer, src); } break;
                case 8: if (Is32Bit) { UnsafeMemory32.WriteRaw8(writer, src); } else { UnsafeMemory64.WriteRaw8(writer, src); } break;
                case 9: if (Is32Bit) { UnsafeMemory32.WriteRaw9(writer, src); } else { UnsafeMemory64.WriteRaw9(writer, src); } break;
                case 10: if (Is32Bit) { UnsafeMemory32.WriteRaw10(writer, src); } else { UnsafeMemory64.WriteRaw10(writer, src); } break;
                case 11: if (Is32Bit) { UnsafeMemory32.WriteRaw11(writer, src); } else { UnsafeMemory64.WriteRaw11(writer, src); } break;
                case 12: if (Is32Bit) { UnsafeMemory32.WriteRaw12(writer, src); } else { UnsafeMemory64.WriteRaw12(writer, src); } break;
                case 13: if (Is32Bit) { UnsafeMemory32.WriteRaw13(writer, src); } else { UnsafeMemory64.WriteRaw13(writer, src); } break;
                case 14: if (Is32Bit) { UnsafeMemory32.WriteRaw14(writer, src); } else { UnsafeMemory64.WriteRaw14(writer, src); } break;
                case 15: if (Is32Bit) { UnsafeMemory32.WriteRaw15(writer, src); } else { UnsafeMemory64.WriteRaw15(writer, src); } break;
                case 16: if (Is32Bit) { UnsafeMemory32.WriteRaw16(writer, src); } else { UnsafeMemory64.WriteRaw16(writer, src); } break;
                case 17: if (Is32Bit) { UnsafeMemory32.WriteRaw17(writer, src); } else { UnsafeMemory64.WriteRaw17(writer, src); } break;
                case 18: if (Is32Bit) { UnsafeMemory32.WriteRaw18(writer, src); } else { UnsafeMemory64.WriteRaw18(writer, src); } break;
                case 19: if (Is32Bit) { UnsafeMemory32.WriteRaw19(writer, src); } else { UnsafeMemory64.WriteRaw19(writer, src); } break;
                case 20: if (Is32Bit) { UnsafeMemory32.WriteRaw20(writer, src); } else { UnsafeMemory64.WriteRaw20(writer, src); } break;
                case 21: if (Is32Bit) { UnsafeMemory32.WriteRaw21(writer, src); } else { UnsafeMemory64.WriteRaw21(writer, src); } break;
                case 22: if (Is32Bit) { UnsafeMemory32.WriteRaw22(writer, src); } else { UnsafeMemory64.WriteRaw22(writer, src); } break;
                case 23: if (Is32Bit) { UnsafeMemory32.WriteRaw23(writer, src); } else { UnsafeMemory64.WriteRaw23(writer, src); } break;
                case 24: if (Is32Bit) { UnsafeMemory32.WriteRaw24(writer, src); } else { UnsafeMemory64.WriteRaw24(writer, src); } break;
                case 25: if (Is32Bit) { UnsafeMemory32.WriteRaw25(writer, src); } else { UnsafeMemory64.WriteRaw25(writer, src); } break;
                case 26: if (Is32Bit) { UnsafeMemory32.WriteRaw26(writer, src); } else { UnsafeMemory64.WriteRaw26(writer, src); } break;
                case 27: if (Is32Bit) { UnsafeMemory32.WriteRaw27(writer, src); } else { UnsafeMemory64.WriteRaw27(writer, src); } break;
                case 28: if (Is32Bit) { UnsafeMemory32.WriteRaw28(writer, src); } else { UnsafeMemory64.WriteRaw28(writer, src); } break;
                case 29: if (Is32Bit) { UnsafeMemory32.WriteRaw29(writer, src); } else { UnsafeMemory64.WriteRaw29(writer, src); } break;
                case 30: if (Is32Bit) { UnsafeMemory32.WriteRaw30(writer, src); } else { UnsafeMemory64.WriteRaw30(writer, src); } break;
                case 31: if (Is32Bit) { UnsafeMemory32.WriteRaw31(writer, src); } else { UnsafeMemory64.WriteRaw31(writer, src); } break;
                default:
                    MemoryCopyUtf8(writer, src);
                    break;
            }
        }

        public static unsafe void MemoryCopyUtf8(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);
#if !NET45
            fixed (void* dstP = &writer.buffer[writer.offset])
            fixed (void* srcP = &src[0])
            {
                Buffer.MemoryCopy(srcP, dstP, writer.buffer.Length - writer.offset, src.Length);
            }
#else
            Buffer.BlockCopy(src, 0, writer.buffer, writer.offset, src.Length);
#endif
            writer.offset += src.Length;
        }
    }

    public static partial class UnsafeMemory32
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw1(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(byte*)pDst = *(byte*)pSrc;
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw2(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(short*)pDst = *(short*)pSrc;
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw3(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(byte*)pDst = *(byte*)pSrc;
                *(short*)(pDst + 1) = *(short*)(pSrc + 1);
            }

            writer.offset += src.Length;
        }
    }

    public static partial class UnsafeMemory64
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw1(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(byte*)pDst = *(byte*)pSrc;
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw2(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(short*)pDst = *(short*)pSrc;
            }

            writer.offset += src.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRaw3(JsonWriterUtf8 writer, byte[] src)
        {
            writer.EnsureCapacity(writer.offset, src.Length);

            fixed (byte* pSrc = &src[0])
            fixed (byte* pDst = &writer.buffer[writer.offset])
            {
                *(byte*)pDst = *(byte*)pSrc;
                *(short*)(pDst + 1) = *(short*)(pSrc + 1);
            }

            writer.offset += src.Length;
        }

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
    }
}

#endif