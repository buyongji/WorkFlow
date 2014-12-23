using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Collections.Generic
{
    internal interface IForWriteList<T> : IEnumerable<T>, IEnumerable
    {
        #region Properties
        int Count { get; }
        T this[int index] { get; set; }
        #endregion

        #region Methods
        IForWriteList<T> Add(T item);
        T[] ToArray();
        #endregion
    }
}
