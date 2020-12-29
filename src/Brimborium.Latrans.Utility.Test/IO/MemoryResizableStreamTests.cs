using System;
using System.Linq;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Xunit;
using System.Collections.Generic;

namespace Brimborium.Latrans.IO {
    public class MemoryResizableStreamTests {
        
        //const int Cnt = 100_000;
        [Theory]
        [InlineData(400)]
        [InlineData(100_000)]
        public void MemoryResizableStream_1Test(int cnt) {
            var sut = new MemoryResizableStream();
            for (int idx = 0; idx < cnt; idx++) {
                sut.WriteByte((byte) (0xff & idx));
            }
            var a = sut.ToArray();
            Assert.Equal(0, a[0]);
            Assert.Equal(1, a[1]);
            Assert.Equal(0, a[256]);
            Assert.Equal(cnt, a.Length);

            sut.Position = 0;
            for (int idx = 0; idx < cnt; idx++) {
                Assert.Equal((byte)idx, sut.ReadByte());
            }
        }
    }
}
