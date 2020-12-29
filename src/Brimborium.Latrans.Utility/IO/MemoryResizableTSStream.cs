using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Brimborium.Latrans.IO {
    /// <summary>
    /// MemoryResizableTSStream is a resizable and threadsafe variant of MemoryStream 
    /// that uses a dynamic list of byte arrays as a backing store, instead of a single byte array, the allocation
    /// of which will fail for relatively small streams as it requires contiguous memory.
    /// </summary>
    public class MemoryResizableTSStream : System.IO.Stream {
        /* http://msdn.microsoft.com/en-us/library/system.io.stream.aspx */
        private long _Length;
        private long _BlockSize;
        private List<byte[]> _Blocks;
        private long _Position;


        public MemoryResizableTSStream(long blockSize = 0) {
            this._Blocks = new List<byte[]>();
            this._Length = 0;
            this._BlockSize = (blockSize < 65536) ? 65536 : blockSize;
            this._Position = 0;
        }

        public MemoryResizableTSStream(byte[] source, long blockSize = 0) {
            this._Blocks = new List<byte[]>();
            this._Length = 0;
            this._BlockSize = (blockSize < 65536) ? 65536 : blockSize;
            this._Position = 0;
            this.Write(source, 0, source.Length);
            this._Position = 0;
        }

        /* length is ignored because capacity has no meaning unless we implement an artifical limit */
        public MemoryResizableTSStream(long length, long blockSize = 0) {
            this._Blocks = new List<byte[]>();
            this._Length = length;
            this._BlockSize = (blockSize < 65536) ? 65536 : blockSize;
            this._Position = 0;
            //access block to prompt the allocation of memory
            this.getBlock(getBlockIdOffset(length).blockId);
        }


        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length => this._Length;

        public override long Position {
            get {
                return this._Position;
            }

            set {
                System.Threading.Interlocked.Exchange(ref this._Position, value);
            }
        }


        /* Use these properties to gain access to the appropriate block of memory for the current Position */

        /// <summary>
        /// The block of memory currently addressed by Position
        /// </summary>
        private byte[] getBlock(int blockId) {
            if (this._Blocks.Count <= blockId) {
                lock (this) {
                    if (this._Blocks.Count <= blockId) {
                        while (this._Blocks.Count <= blockId) {
                            this._Blocks.Add(new byte[this._BlockSize]);
                        }
                        System.Threading.Interlocked.MemoryBarrier();
                    }
                }
            }
            return this._Blocks[blockId];
        }

#if false
        /// <summary>
        /// The id of the block currently addressed by Position
        /// </summary>
        private int getBlockId() => (int)(this._Position / this._BlockSize);

        /// <summary>
        /// The offset of the byte currently addressed by Position, into the block that contains it
        /// </summary>
        private long getBlockOffset() => this._Position % this._BlockSize;
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private (int blockId, long blockOffset) getBlockIdOffset(long position)
            => ((int)(this._Position / this._BlockSize), this._Position % this._BlockSize);

        public override void Flush() {
        }

        public override int Read(byte[] buffer, int offset, int count) {
            if (buffer == null) {
                throw new ArgumentNullException("buffer", "Buffer cannot be null.");
            }
            if (offset < 0) {
                throw new ArgumentOutOfRangeException("offset", offset, "Destination offset cannot be negative.");
            }
            lock (this) {
                long lcount = (long)count;

                if (lcount < 0) {
                    throw new ArgumentOutOfRangeException("count", lcount, "Number of bytes to copy cannot be negative.");
                }

                long position = this._Position;
                long remaining = (this._Length - position);
                if (lcount > remaining) {
                    lcount = remaining;
                }

                int read = 0;
                do {
                    var (blockId, blockOffset) = this.getBlockIdOffset(position);
                    long copysize = Math.Min(lcount, (this._BlockSize - blockOffset));
                    var lclBlock = this.getBlock(blockId);
                    Array.Copy(lclBlock, blockOffset, buffer, offset, copysize);
                    lcount -= copysize;
                    offset += (int)copysize;
                    read += (int)copysize;
                    position += copysize;
                } while (lcount > 0);
                System.Threading.Interlocked.Exchange(ref this._Position, position);

                return read;
            }
        }

        public override long Seek(long offset, SeekOrigin origin) {
            switch (origin) {
                case SeekOrigin.Begin:
                    System.Threading.Interlocked.Exchange(ref this._Position, offset);
                    break;
                case SeekOrigin.Current:
                    System.Threading.Interlocked.Add(ref this._Position, offset);
                    break;
                case SeekOrigin.End:
                    System.Threading.Interlocked.Exchange(ref this._Position, this.Length - offset);
                    break;
            }
            return this._Position;
        }

        public override void SetLength(long value) {
            lock (this) {
                this._Length = value;
            }
        }

        public override void Write(byte[] buffer, int offset, int count) {
            lock (this) {
                long lCount = (long)count;
                long lOffset = (long)offset;
                try {
                    long position = this._Position;
                    do {
                        var (blockId, blockOffset) = this.getBlockIdOffset(position);
                        long copysize = Math.Min(lCount, this._BlockSize - blockOffset);

                        this.EnsureCapacity(position + copysize);
                        var lclBlock = this.getBlock(blockId);
                        Array.Copy(buffer, lOffset, lclBlock, blockOffset, copysize);
                        lCount -= copysize;
                        lOffset += copysize;
                        position += copysize;
                    } while (lCount > 0);
                    System.Threading.Interlocked.Exchange(ref this._Position, position);
                } catch {
                    throw;
                }
            }
        }

        public override int ReadByte() {
            byte b;
            lock (this) {
                if (this._Position >= this._Length) {
                    return -1;
                }
                var (blockId, blockOffset) = getBlockIdOffset(this._Position);
                var lclBlock = this.getBlock(blockId);
                b = lclBlock[blockOffset];
                System.Threading.Interlocked.Increment(ref this._Position);
            }
            return b;
        }

        public override void WriteByte(byte value) {
            lock (this) {
                this.EnsureCapacity(this._Position + 1);
                var (blockId, blockOffset) = getBlockIdOffset(this._Position);
                var lclBlock = this.getBlock(blockId);
                lclBlock[blockOffset] = value;
                System.Threading.Interlocked.Increment(ref this._Position);
            }
        }

        private void EnsureCapacity(long intended_length) {
            if (intended_length > this._Length)
                this._Length = (intended_length);
        }

#if false
        /* http://msdn.microsoft.com/en-us/library/fs2xkftw.aspx */
        protected override void Dispose(bool disposing) {
            /* We do not currently use unmanaged resources */
            base.Dispose(disposing);
        }
#endif

        /// <summary>
        /// Returns the entire content of the stream as a byte array. This is not safe because the call to new byte[] may 
        /// fail if the stream is large enough. Where possible use methods which operate on streams directly instead.
        /// </summary>
        /// <returns>A byte[] containing the current data in the stream</returns>
        public byte[] ToArray() {
            lock (this) {
                long length = this._Length;
                byte[] destination = new byte[length];
                long offset = 0;
                foreach (var lclBlock in this._Blocks) {
                    var copysize = Math.Min(length, this._BlockSize);
                    //Buffer.BlockCopy(lclBlock, 0, destination, (int)offset, (int)copysize);
                    Array.Copy(lclBlock, 0, destination, offset, copysize);
                    offset += (int)copysize;
                    length -= copysize;
                }
                return destination;
            }
        }

        /// <summary>
        /// Reads length bytes from source into the this instance at the current position.
        /// </summary>
        /// <param name="source">The stream containing the data to copy</param>
        /// <param name="length">The number of bytes to copy</param>
        public void ReadFrom(Stream source, long length) {
            byte[] buffer = new byte[4096];
            int read;
            do {
                read = source.Read(buffer, 0, (int)Math.Min(4096, length));
                length -= read;
                this.Write(buffer, 0, read);

            } while (length > 0);
        }

        /// <summary>
        /// Writes the entire stream into destination, regardless of Position, which remains unchanged.
        /// </summary>
        /// <param name="destination">The stream to write the content of this stream to</param>
        public void WriteTo(Stream destination) {
            lock (this) {
                long initialpos = this.Position;
                this.Position = 0;
                this.CopyTo(destination);
                this.Position = initialpos;
            }
        }
    }
}
