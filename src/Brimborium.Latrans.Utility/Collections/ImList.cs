using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Brimborium.Latrans.Collections {
    public sealed class ImList<T>
        : IEnumerable<T>
        where T : class {
        private static ImList<T>? _Empty;
        public static ImList<T> Empty => (_Empty ??= new ImList<T>());

        private int _Count;
        private readonly Node[] _NodeItems;

        private ImList(Node[] nodeItems) {
            this._NodeItems = nodeItems;
            this._Count = -1;
        }

        public ImList() {
            this._NodeItems = Array.Empty<Node>();
            this._Count = 0;
        }

        public ImList(IEnumerable<T>? source) {
            if (source is null) {
                this._NodeItems = Array.Empty<Node>();
                this._Count = 0;
            } else if (source is ICollection<T> collection) {
                var items = new T[collection.Count];
                collection.CopyTo(items, 0);
                this._NodeItems = new Node[1] { new Node(items) };
                this._Count = this._NodeItems[0].Length;
                //this._Nodes = new ImList2(items);
            } else if (source is T[] array) {
                var items = new T[array.Length];
                Array.Copy(array, 0, items, 0, array.Length);
                this._NodeItems = new Node[1] { new Node(items) };
                this._Count = this._NodeItems[0].Length;
            } else {
                var items = source.ToArray();
                this._NodeItems = new Node[1] { new Node(items) };
                this._Count = this._NodeItems[0].Length;
            }
        }

        public ImList(T[] array) {
            var items = new T[array.Length];
            Array.Copy(array, 0, items, 0, array.Length);
            this._NodeItems = new Node[1] { new Node(items) };
            this._Count = this._NodeItems[0].Length;
        }

        public ImList<T> Add(T item) {
            var nodesLength = this._NodeItems.Length;
            if (nodesLength == 0) {
                return new ImList<T>(new Node[1] { new Node(item) });
            } else if (nodesLength<256) {
                var nodeLast = this._NodeItems[nodesLength - 1];
                if (nodeLast.Length < 8) {
                    return new ImList<T>(
                        ArrayCopySetLast(
                            this._NodeItems,
                            nodeLast.Add(item)));
                } else {
                    return new ImList<T>(
                        ArrayCopyAdd(
                            this._NodeItems,
                            new Node(item)));
                }
            }
#if false
            {
                
                var itemsLength = this._NodeItems.Length;
                for (int idx = idxLow; idx < itemsLength; idx++) {
                    if (this._NodeItems[idx].CanAdd()) {
                        var nodeA = this._NodeItems[idx].Add(item);
                        var items = new Node[itemsLength];
                        Array.Copy(this._NodeItems, 0, items, 0, itemsLength);
                        items[idx] = nodeA;
                        if (idx == this._CountUsed) {
                            return new ImList<T>(items, this._CountUsed + 1);
                        } else {
                            return new ImList<T>(items, this._CountUsed);
                        }
                    }
                }
            }
#endif
            {
                var count = this.Count;
                var itemsOld = new T[count];
                this.ToArray(0, itemsOld);
                return new ImList<T>(new Node[2] { new Node(itemsOld), new Node(item) });
            }
        }

        public T this[int index] {
            get {
                var (node, idxItem) = this.FindNodeIndex(index);
                if (node is null) {
                    throw new IndexOutOfRangeException($"0<={index}<{this.Count}");
                } else {
                    return node.Items[idxItem];
                }
            }
        }

        private (Node? node, int idxItem) FindNodeIndex(int index) {
            if (index < 0) {
                return (null, -1);
            } else {
                var sumCount = 0;
                for (int idxNode = 0; idxNode < this._NodeItems.Length; idxNode++) {
                    //
                    var node = this._NodeItems[idxNode];
                    var length = node.Items.Length;
                    var idxItem = index - sumCount;
                    //
                    if (0 <= index && idxItem < length) {
                        return (node, idxItem);
                    } else {
                        sumCount += length;
                    }
                }
                return (null, 0);
            }
        }

        public T[] ToArray() {
            var count = this.GetCount(0);
            var dst = new T[count];
            this.ToArray(0, dst);
            return dst;
        }

        internal void ToArray(int fromIdx, T[] dst) {
            int idxNext = 0;
            for (int idx = fromIdx; idx < this._NodeItems.Length; idx++) {
                this._NodeItems[idx].ToArray(ref idxNext, dst);
            }
        }

        public bool IsEmpty() {
            for (int idx = 0; idx < this._NodeItems.Length; idx++) {
                if (this._NodeItems[idx].Length > 0) {
                    return false;
                }
            }
            return true;
        }

        public int Count => this.GetCount(0);

        internal int GetCount(int idxFrom) {
            if ((this._Count < 0) && (idxFrom == 0)) {
                return this._Count;
            } else {
                int result = 0;
                for (int i = idxFrom; i < this._NodeItems.Length; i++) {
                    result += this._NodeItems[i].Length;
                }

                if (idxFrom == 0) {
                    // cache only if complete
                    return this._Count = result;
                } else {
                    return result;
                }
            }
        }

        public IEnumerator<T> GetEnumerator() {
            for (int i1 = 0; i1 < this._NodeItems.Length; i1++) {
                var items1 = this._NodeItems[i1];
                for (int i2 = 0; i2 < items1.Length; i2++) {
                    yield return items1.Items[i2];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        internal sealed class Node {
            private static Node? _Empty;
            public static Node Empty => (_Empty ??= new Node());

            public readonly T[] Items;
            public int Length => this.Items.Length;

            public Node() {
                this.Items = Array.Empty<T>();
            }

            public Node(T item) {
                this.Items = new T[1] { item };
            }

            public Node(T[] items) {
                this.Items = items;
            }

            public Node(T[] items, T item) {
                this.Items = ArrayCopyAdd(items, item);
            }

            public Node Add(T item) {
                return new Node(ArrayCopyAdd(this.Items, item));
            }

            public bool CanAdd() => (
                (this.Length < this.Items.Length)
                || (this.Items.Length == 0));

            internal void ToArray(ref int idxNext, T[] dst) {
                var itemsLength = this.Items.Length;
                Array.Copy(this.Items, 0, dst, idxNext, itemsLength);
                idxNext += itemsLength;
            }
        }

        private static S[] ArrayCopySetLast<S>(S[] items, S item)
            where S : class {
            var length = items.Length;
            if (length == 0) {
                return new S[1] { item };
            } else {
                var result = new S[length];
                if ((length - 1) > 0) { 
                    Array.Copy(items, 0, result, 0, length-1);
                }
                result[length - 1] = item;

                return result;
            }
        }

        private static S[] ArrayCopyAdd<S>(S[] items, S item)
            where S : class {
            var length = items.Length;
            if (length == 0) {
                return new S[] { item };
            } else {
                var result = new S[length + 1];
                Array.Copy(items, 0, result, 0, length);
                result[length] = item;
                return result;
            }
        }
    }
}
