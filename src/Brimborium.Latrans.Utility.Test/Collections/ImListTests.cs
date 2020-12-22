using System;
using System.Linq;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Xunit;
using System.Collections.Generic;
// Brimborium.Latrans.Benchmark
namespace Brimborium.Latrans.Collections {
    public class ImListTests {
        [Fact]
        public void ImList_1_Add() {
            var l0 = new ImList<D>();
            var l1 = l0.Add(new D(1));
            var l2 = l1.Add(new D(2));
            var l10 = l2.Add(new D(3)).Add(new D(4)).Add(new D(5)).Add(new D(6)).Add(new D(7)).Add(new D(8)).Add(new D(9)).Add(new D(10));
            Assert.Equal(0, l0.Count);
            Assert.Equal(1, l1.Count);
            Assert.Equal(10, l10.Count);

            Assert.Equal(new D[] { }, l0.ToList(), new DEqualityComparer());
            Assert.Equal(new D[] { }, l0.ToArray(), new DEqualityComparer());

            Assert.Equal(new D[] { new D(1) }, l1.ToList(), new DEqualityComparer());
            Assert.Equal(new D[] { new D(1) }, l1.ToArray(), new DEqualityComparer());

            Assert.Equal(new D[] { new D(1), new D(2) }, l2.ToList(), new DEqualityComparer());
            Assert.Equal(new D[] { new D(1), new D(2) }, l2.ToArray(), new DEqualityComparer());

            Assert.Equal(new D[] {
                new D(1), new D(2) , new D(3), new D(4), new D(5), new D(6), new D(7), new D(8), new D(9), new D(10)
                }, 
                l10.ToArray(), 
                new DEqualityComparer());
            //BenchmarkDotNet
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(8 * 8)]
        [InlineData(8 * 8 + 1)]
        [InlineData(1000)]
        public void ImList_2_Add(int cnt) {
            var l = new ImList<D>();
            for (int idx = 0; idx < cnt; idx++) {
                l = l.Add(new D(idx+1));
            }
            Assert.Equal(cnt, l.Count);
            var arr = l.ToArray();
            Assert.Equal(cnt, arr.Length);
            for (int idx = 0; idx < cnt; idx++) {
                Assert.Equal(idx+1, arr[idx].Data1);
            }
        }

        [Fact]
        public void ImList_3_Add() {
            var l = new ImList<D>();
            for (int idx = 0; idx < 8 * 8; idx++) {
                l = l.Add(new D(idx+1));
                Assert.NotNull(l);
                Assert.NotNull(l.ToArray()[l.Count - 1]);
                Assert.Equal(idx + 1, l.ToArray()[l.Count - 1].Data1);
            }
            
            l = l.Add(new D(-1));
            Assert.NotNull(l);
            Assert.NotNull(l.ToArray()[l.Count - 1]);
            Assert.Equal(-1, l.ToArray()[l.Count - 1].Data1);
            
            for (int idx = 0; idx < 55; idx++) {
                l = l.Add(new D(idx + 1));
                Assert.NotNull(l);
                Assert.NotNull(l.ToArray()[l.Count - 1]);
                Assert.Equal(idx + 1, l.ToArray()[l.Count - 1].Data1);
            }

            l = l.Add(new D(-2));
            Assert.NotNull(l);
            Assert.NotNull(l.ToArray()[l.Count - 1]);
            Assert.Equal(-2, l.ToArray()[l.Count - 1].Data1);

            //Assert.Equal(64+1+64, l.Count);
        }

        class D {
            [DebuggerStepThrough]
            public D(int data1) {
                this.Data1 = data1;
            }
            public int Data1 { get; }
            public override string ToString()
                => this.Data1.ToString();
        }

        class DEqualityComparer : IEqualityComparer<D> {
            public bool Equals([AllowNull] D x, [AllowNull] D y) {
                if (ReferenceEquals(x, y)) { return true; }
                if (x is null || y is null) { return false; }
                return x.Data1 == y.Data1;
            }

            public int GetHashCode([DisallowNull] D obj) {
                return obj.Data1;
            }
        }
    }
}
