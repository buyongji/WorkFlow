using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Collections.Generic
{
    [Serializable, DebuggerTypeProxy(typeof(ForWriteListDebugView<>)), DebuggerDisplay("Count = {Count}")]
    public sealed class ForWriteList<T> : ICollection<T>, IEnumerable<T>, IEnumerable
    {
        #region Fields
        private T[] _currentArray;
        private int _currentArrayPtr;
        private Pair<T> _lastPair;
        private int _pairsArrayLength;
        internal static readonly T[] Empty;
        #endregion

        #region Properties
        public int Count
        {
            get
            {
                return (this._currentArrayPtr + this._pairsArrayLength);
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        #endregion
        #region Methods
        static ForWriteList()
        {
            ForWriteList<T>.Empty = new T[0];
        }

        public ForWriteList()
        {
            this._currentArray = ForWriteList<T>.Empty;
        }

        internal ForWriteList(T one, T two)
        {
            this._currentArray = new T[] { one, two };
            this._currentArrayPtr = 2;
        }

        public void Add(T item)
        {
            if (this._currentArrayPtr == this._currentArray.Length)
            {
                this.EnsureCapacity();
            }
            this._currentArray[this._currentArrayPtr++] = item;
        }

        public void AddRange(IEnumerable collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (collection is ICollection)
            {
                ICollection is2 = (ICollection)collection;
                if (is2.Count > 0)
                {
                    if (is2.Count > (this._currentArray.Length - this._currentArrayPtr))
                    {
                        if (this._currentArray.Length == 0)
                        {
                            this._currentArray = new T[is2.Count];
                        }
                        else
                        {
                            Array.Resize<T>(ref this._currentArray, this._currentArrayPtr + is2.Count);
                        }
                        is2.CopyTo(this._currentArray, this._currentArrayPtr);
                        this._currentArrayPtr += is2.Count;
                        this.EnsureCapacity();
                    }
                    else
                    {
                        is2.CopyTo(this._currentArray, this._currentArrayPtr);
                        this._currentArrayPtr += is2.Count;
                    }
                }
            }
            else
            {
                foreach (object obj2 in collection)
                {
                    this.Add((T)obj2);
                }
            }
        }

        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (collection is ICollection<T>)
            {
                ICollection<T> is2 = (ICollection<T>)collection;
                if (is2.Count > 0)
                {
                    if (is2.Count > (this._currentArray.Length - this._currentArrayPtr))
                    {
                        if (this._currentArray.Length == 0)
                        {
                            this._currentArray = new T[is2.Count];
                        }
                        else
                        {
                            Array.Resize<T>(ref this._currentArray, this._currentArrayPtr + is2.Count);
                        }
                        is2.CopyTo(this._currentArray, this._currentArrayPtr);
                        this._currentArrayPtr += is2.Count;
                        this.EnsureCapacity();
                    }
                    else
                    {
                        is2.CopyTo(this._currentArray, this._currentArrayPtr);
                        this._currentArrayPtr += is2.Count;
                    }
                }
            }
            else
            {
                foreach (T local in collection)
                {
                    this.Add(local);
                }
            }
        }

        private void EnsureCapacity()
        {
            int length = this._currentArray.Length;
            if (length > 0)
            {
                Pair<T> pair = new Pair<T>
                {
                    Array = this._currentArray,
                    Previous = this._lastPair
                };
                this._lastPair = pair;
                this._pairsArrayLength += length;
            }
            length = (length == 0) ? 4 : (length * 2);
            this._currentArray = new T[length];
            this._currentArrayPtr = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (this._currentArray.Length > 0)
            {
                List<KeyValuePair<int, T[]>> iteratorVariable0 = new List<KeyValuePair<int, T[]>>(4) {
                new KeyValuePair<int, T[]>(this._currentArrayPtr, this._currentArray)
            };
                for (Pair<T> iteratorVariable1 = this._lastPair; iteratorVariable1 != null; iteratorVariable1 = iteratorVariable1.Previous)
                {
                    iteratorVariable0.Add(new KeyValuePair<int, T[]>(iteratorVariable1.Array.Length, iteratorVariable1.Array));
                }
                for (int i = iteratorVariable0.Count - 1; i >= 0; i--)
                {
                    KeyValuePair<int, T[]> iteratorVariable2 = iteratorVariable0[i];
                    for (int j = 0; j < iteratorVariable2.Key; j++)
                    {
                        yield return iteratorVariable2.Value[j];
                    }
                }
            }
        }

        void ICollection<T>.Clear()
        {
            this._currentArray = ForWriteList<T>.Empty;
            this._lastPair = new Pair<T>();
            this._pairsArrayLength = 0;
            this._currentArrayPtr = 0;
        }

        bool ICollection<T>.Contains(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            if (this._currentArray.Length != 0)
            {
                int destinationIndex = (this.Count - this._currentArrayPtr) + arrayIndex;
                Array.Copy(this._currentArray, 0, array, destinationIndex, this._currentArrayPtr);
                for (Pair<T> pair = this._lastPair; pair != null; pair = pair.Previous)
                {
                    destinationIndex -= pair.Array.Length;
                    pair.Array.CopyTo(array, destinationIndex);
                }
            }
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public T[] ToArray()
        {
            if (this._currentArray.Length == 0)
            {
                return ForWriteList<T>.Empty;
            }
            T[] destinationArray = new T[this.Count];
            int destinationIndex = destinationArray.Length - this._currentArrayPtr;
            Array.Copy(this._currentArray, 0, destinationArray, destinationIndex, this._currentArrayPtr);
            for (Pair<T> pair = this._lastPair; pair != null; pair = pair.Previous)
            {
                destinationIndex -= pair.Array.Length;
                pair.Array.CopyTo(destinationArray, destinationIndex);
            }
            return destinationArray;
        }

        [Serializable]
        private sealed class Pair
        {
            public T[] Array;
            public ForWriteList<T>.Pair Previous;
            public Pair();
        }
        #endregion


    }




}
