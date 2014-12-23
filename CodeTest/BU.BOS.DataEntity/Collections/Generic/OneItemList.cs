using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BU.BOS.Collections.Generic
{
    internal struct OneItemList<T> : IForWriteList<T>, IEnumerable<T>, IEnumerable
    {
        private T _firstItem;

        #region Methods
        public OneItemList(T item)
        {
            this._firstItem = item;
        }
        public IForWriteList<T> Add(T item)
        {
            throw new NotImplementedException();
        }
        public T this[int index]
        {
            get
            {
                if(index!=0)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return this._firstItem;
            }
            set
            {
                if(index!= 0)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                this._firstItem = value;
            }
        }
        public int Count
        {
            get
            {
                return 0;
            }
        }
        public T[] ToArray()
        {
            return new T[] { this._firstItem };
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ListEnumerator<T>((OneItemList<T>)this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}
