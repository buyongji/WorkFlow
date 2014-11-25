using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BU.BOS.Collections.Generic
{
    public interface IKeyedCollectionBase<TKey, TValue> : IEnumerable<TValue>, IEnumerable
    {
        bool Contains(TValue item);
        bool ContainsKey(TKey key);
        void CopyTo(TValue[] array, int arrayIndex);
        int IndexOf(TValue item);
        TValue[] ToArray();
        bool TryGetValue(TKey key, out TValue value);

        // Properties
        int Count { get; }
        TValue this[int index] { get; }
        TValue this[TKey key] { get; }

    }
}
