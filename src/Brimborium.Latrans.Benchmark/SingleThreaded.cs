using System;
using System.Collections.Generic;
using System.Linq;

using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;

using Brimborium.Latrans.Collections;

namespace Benchmark {
    public class SingleThreaded {
        const int cntAdd = 8 * 8 * 8;
        const int cntRead = 100;

        [Benchmark(Baseline = true)]
        public void Scenario1List() {
            var l = new List<D>();
            for (int idx = 1; idx <= cntAdd; idx++) {
                l.Add(new D(idx));
            }
            for (int idx = 1; idx <= cntRead; idx++) {
                if (l.ToArray().Length != cntAdd) { throw new Exception(); }
            }
        }

        [Benchmark()]
        public void Scenario2ListCopied() {
            var l = new List<D>();
            for (int idx = 1; idx <= cntAdd; idx++) {
                l = l.ToList();
                l.Add(new D(idx));
            }
            for (int idx = 1; idx <= cntRead; idx++) {
                if (l.ToArray().Length != cntAdd) { throw new Exception($"{l.ToArray().Length} != {cntAdd}"); }
            }
        }

        [Benchmark]
        public void Scenario3ImmutableList() {
            var l = System.Collections.Immutable.ImmutableList<D>.Empty;
            for (int idx = 1; idx <= cntAdd; idx++) {
                l = l.Add(new D(idx));
            }
            for (int idx = 1; idx <= cntRead; idx++) {
                if (l.ToArray().Length != cntAdd) { throw new Exception($"{l.ToArray().Length} != {cntAdd}"); }
            }
        }

        [Benchmark]
        public void Scenario4ImList() {
            var l = new ImList<D>();
            for (int idx = 1; idx <= cntAdd; idx++) {
                l = l.Add(new D(idx));
            }
            for (int idx = 1; idx <= cntRead; idx++) {
                if (l.ToArray().Length != cntAdd) { throw new Exception($"{l.ToArray().Length} != {cntAdd}"); }
            }
        }

        class D {
            public D(int data1) {
                this.Data1 = data1;
            }
            public int Data1 { get; }
            public override string ToString()
                => this.Data1.ToString();
        }
    }
}
