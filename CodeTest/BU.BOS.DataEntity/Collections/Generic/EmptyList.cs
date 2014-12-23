using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Collections.Generic
{
    internal sealed class EmptyList<T> : IForWriteList<T>, IEnumerable<T>, IEnumerable
    {
        #region Properties
        public int Count
        {
            get
            {
                return 0;
            }
        }

        public T this[int index]
        {
            get
            {
                throw new ArgumentOutOfRangeException("index");
            }
            set
            {
                throw new ArgumentOutOfRangeException("index");
            }
        }
        #endregion

        #region Methods
        public IForWriteList<T> Add(T item)
        {
            return new OneItemList<T>(item);
        }

        public T[] ToArray()
        {
            return ForWriteList<T>.Empty;
        }
        #endregion
    }
}
