using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Brimborium.Latrans.Collections {
    public sealed class ImList<T>
        : IEnumerable<T>
        where T : class {
        private static ImList<T>? _Empty;
        public static ImList<T> Empty => (_Empty ??= new ImList<T>());

        private static ImList<T>.Node[] getEmptyNodes() {
            var e = Node.Empty;
            ImList<T>.Node[] result = new Node[] { e, e, e, e, e, e, e, e };
            return result;
        }

        private static ImList<T>.Node[] getEmptyNodes(T[] items) {
            var e = Node.Empty;
            ImList<T>.Node[] result = new Node[] { new Node(items), e, e, e, e, e, e, e };
            return result;
        }

        private static ImList<T>.Node[] getEmptyNodes(T[] items , T item) {
            var e = Node.Empty;
            ImList<T>.Node[] result = new Node[] { new Node(items), new Node(item), e, e, e, e, e, e };
            return result;
        }

        private int _Count;
        private readonly Node[] _NodeItems;
        private readonly int _CountUsed;

        private ImList(Node[] nodeItems, int countUsed) {
            if (nodeItems.Length < 8) {
                var correctedNodeItems = getEmptyNodes();
                Array.Copy(nodeItems, 0, correctedNodeItems, 0, nodeItems.Length);
                this._NodeItems = correctedNodeItems;
            } else { 
                this._NodeItems = nodeItems;
            }
            this._CountUsed = countUsed;
        }

        public ImList() {
            this._NodeItems = getEmptyNodes();
            this._CountUsed = 0;
        }

        public ImList(IEnumerable<T>? source) {
            var e = Node.Empty;
            if (source is null) {
                this._NodeItems = new Node[8] { e, e, e, e, e, e, e, e };
                this._CountUsed = 0;
            } else if (source is ICollection<T> collection) {
                var items = new T[collection.Count];
                collection.CopyTo(items, 0);
                this._NodeItems = new Node[8] { new Node(items), e, e, e, e, e, e, e };
                this._CountUsed = 1;
                //this._Nodes = new ImList2(items);
            } else if (source is T[] array) {
                var items = new T[array.Length];
                Array.Copy(array, 0, items, 0, array.Length);
                this._NodeItems = new Node[8] { new Node(items), e, e, e, e, e, e, e };
                this._CountUsed = 1;
            } else {
                var items = source.ToArray();
                this._NodeItems = new Node[8] { new Node(items), e, e, e, e, e, e, e };
                this._CountUsed = 1;
            }
        }

        public ImList(T[] items) {
            this._NodeItems = getEmptyNodes(items);
            this._CountUsed = 1;
        }

        public ImList<T> Add(T item) {
            {
                var idxLow = (this._CountUsed == 0) ? 0 : (this._CountUsed - 1);
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
            {
                var count = this.GetCount(0);
                var items0 = new T[count];
                this.ToArray(0, items0);
                return new ImList<T>(getEmptyNodes(items0, item), 2);
            }
        }

        public T[] ToArray() {
            var count = this.GetCount(0);
            var dst = new T[count];
            this.ToArray(0, dst);
            return dst;
        }

        internal void ToArray(int fromIdx,  T[] dst) {
            int idxNext = 0;
            for (int idx = fromIdx; idx < this._CountUsed; idx++) {
                this._NodeItems[idx].ToArray(ref idxNext, dst);
            }
        }

        public bool IsEmpty() {
            for (int idx = 0; idx < this._CountUsed; idx++) {
                if (this._NodeItems[idx].CountUsed > 0) {
                    return false;
                }
            }
            return true;
        }

        public int Count => this.GetCount(0);

        internal int GetCount(int idxFrom) {
            if ((this._Count > 0) && (idxFrom == 0)) {
                return this._Count;
            } else { 
                int result = 0;
                for (int i = idxFrom; i < this._CountUsed; i++) {
                    result += this._NodeItems[i].CountUsed;
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
            for (int i1 = 0; i1 < this._CountUsed; i1++) {
                var items1 = this._NodeItems[i1];
                for (int i2 = 0; i2 < items1.CountUsed; i2++) {
                    yield return items1.Items[i2];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        internal sealed class Node {
            private static Node? _Empty;
            public static Node Empty => (_Empty ??= new Node());

            public readonly T[] Items;
            public readonly int CountUsed;

            public Node() {
                this.Items = Array.Empty<T>();
                this.CountUsed = 0;
            }

            public Node(T item) {
                var items = new T[8];
                items[0] = item;
                this.Items = items;
                this.CountUsed = 1;
            }

            public Node(T[] items) {
                this.Items = items;
                this.CountUsed = items.Length;
            }

            public Node(T[] items, int count) {
                this.Items = items;
                this.CountUsed = count;
            }

            public Node Add(T item) {
                var itemsLength = this.Items.Length;
                if (this.CountUsed < this.Items.Length) {
                    var items = new T[itemsLength];
                    Array.Copy(this.Items, 0, items, 0, itemsLength);
                    items[this.CountUsed] = item;
                    return new Node(items, this.CountUsed + 1);
                } else {
                    var items = new T[itemsLength + 8];
                    Array.Copy(this.Items, 0, items, 0, itemsLength);
                    items[this.CountUsed] = item;
                    return new Node(items, this.CountUsed + 1);
                }
            }

            public bool CanAdd() => (
                (this.CountUsed < this.Items.Length)
                || (this.Items.Length == 0));

            internal void ToArray(ref int idxNext, T[] dst) {
                Array.Copy(this.Items, 0, dst, idxNext, this.CountUsed);
                idxNext += this.CountUsed;
            }
        }
    }
}
