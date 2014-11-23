using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Collections.Generic
{
    public interface IKeyedCollectionBase<TKey, TValue> : IEnumerable<TValue>, IEnumerable
    {
        #region Properties
        int Count { get; }
        TValue this[int index] { get; }
        TValue this[TKey key] { get; }
        #endregion

        #region Methods
        bool Contains(TValue item);
        bool ContainsKey(TKey key);
        void CopyTo(TValue[] array, int arrayIndex);
        int IndexOf(TValue item);
        TValue[] ToArray();
        bool TryGetValue(TKey key, out TValue value);
#endregion

    }
}
