using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace BU.BOS.Collections.Generic
{
    [Serializable]
    public abstract class KeyedCollectionBase<TKey, TValue> : ReadOnlyCollection<TValue>, IKeyedCollectionBase<TKey, TValue>, IEnumerable<TValue>, IEnumerable, IDeserializationCallback
    {
        #region Fields
        [NonSerialized]
        private IEqualityComparer<TKey> _comparer;
        [NonSerialized]
        private Dictionary<TKey, TValue> _dict;
        #endregion

        #region Properties
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
        #endregion

        #region Methods
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
            this._dict = new Dictionary<TKey, TValue>(this._comparer);
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
        #endregion
    }
}
