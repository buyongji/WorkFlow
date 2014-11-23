using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Collections.Generic
{
    [Serializable]
    public abstract class KeyedCollectionBase<TKey, TValue> : IReadOnlyCollection<TValue>, IKeyedCollectionBase<TKey, TValue>, IEnumerable<TValue>, IEnumerable, IDeserializationCallback
    {
        // Fields
        [NonSerialized]
        private IEqualityComparer<TKey> _comparer;
        [NonSerialized]
        private Dictionary<TKey, TValue> _dict;

        // Methods
        protected KeyedCollectionBase(IList<TValue> list)
            : base(list)
        {
            this._comparer = EqualityComparer<TKey>.Default;
            this.CreateDictionary();
        }

        public bool ContainsKey(TKey key)
        {
            TValue local;
            return this.TryGetValue(key, out local);
        }

        private void CreateDictionary()
        {
            this._dict = new Dictionary<TKey, TValue>(this.Comparer);
            foreach (TValue local in base.Items)
            {
                TKey keyForItem = this.GetKeyForItem(local);
                if (keyForItem != null)
                {
                    this._dict.Add(keyForItem, local);
                }
            }
        }

        protected abstract TKey GetKeyForItem(TValue item);
        TValue IKeyedCollectionBase<TKey, TValue>.get_Item(int num1)
        {
            return base[num1];
        }

        public virtual void OnDeserialization(object sender)
        {
            this._comparer = EqualityComparer<TKey>.Default;
            this.CreateDictionary();
        }

        public TValue[] ToArray()
        {
            TValue[] array = new TValue[base.Count];
            base.CopyTo(array, 0);
            return array;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return this._dict.TryGetValue(key, out value);
        }

        // Properties
        protected virtual IEqualityComparer<TKey> Comparer
        {
            get
            {
                return this._comparer;
            }
        }

        protected IDictionary<TKey, TValue> Dictionary
        {
            get
            {
                return this._dict;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue local;
                if (!this.TryGetValue(key, out local))
                {
                    throw new KeyNotFoundException();
                }
                return local;
            }
        }

    }
}
