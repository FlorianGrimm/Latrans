using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using BenchmarkDotNet.Attributes;

namespace Benchmark {
    public class ArrayCloneTest {
        #region Setup code

#if UseRandom
        private readonly Random _Random = new Random();
#endif

        public int[] Data { get; set; } = new int[0];

        [Params(0, 6, 2000)]
        public int N;

        [GlobalSetup]
        public void GlobalSetup() {
#if UseRandom
            this.Data = new int[N]; // executed once per each N value
            for (int i = 0; i < N; i++) {
                this.Data[i] = _Random.Next();
            }
#else
            this.Data = new int[N]; // executed once per each N value
            for (int i = 0; i < N; i++) {
                this.Data[i] = int.MaxValue - i;
            }
#endif
            System.Threading.Interlocked.MemoryBarrier();
        }

        #endregion


        [Benchmark]
        public int[] ToArray() {
            return Data.ToArray();
        }

        [Benchmark]
        public int[] ForLoop() {
            var data = this.Data;

            var copy = new int[data.Length];
            for (int i = 0; i < data.Length; i++) {
                copy[i] = data[i];
            }

            return copy;
        }


        [Benchmark]
        public int[] ForEach() {
            var data = this.Data;

            var copy = new int[data.Length];

            int i = 0;
            foreach (var item in data) {
                copy[i++] = item;
            }

            return copy;
        }

        [Benchmark]
        public int[] Clone() {
            var data = this.Data;
            return (int[])data.Clone();
        }


        [Benchmark]
        public int[] ArrayCopy() {
            var data = this.Data;
            var copy = new int[data.Length];
            Array.Copy(data, copy, data.Length);
            return copy;
        }

        [Benchmark]
        public int[] SpanToArray() {
            var data = this.Data;
            return data.AsSpan().ToArray();
        }

        [Benchmark]
        public int[] ReadOnlySpanToArray() {
            var data = this.Data;
            return new ReadOnlySpan<int>(data).ToArray();
        }

    }
}
