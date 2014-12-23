using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BU.BOS.Collections.Generic
{
    internal struct ListEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator
    {
        private IForWriteList<T> _list;
        private int _index;
        private T _current;
        public ListEnumerator(IForWriteList<T> list)
        {
            this._list = list;
            this._index = 0;
            this._current = default(T);
        }

        public T Current
        {
            get
            {
                return this._current;
            }
        }
        public void Dispose()
        {
        }

        object IEnumerator.Current
        {
            get
            {
                return this._current;
            }
        }
        public bool MoveNext()
        {
            IForWriteList<T> list = this._list;
            if (this._index < list.Count)
            {
                this._current = list[this._index];
                this._index++;
                return true;
            }
            return this.MoveNextRare();
        }

        private bool MoveNextRare()
        {
            this._index = this._list.Count + 1;
            this._current = default(T);
            return false;
        }

        public void Reset()
        {
            this._index = 0;
            this._current = default(T);
        }
    }
}
