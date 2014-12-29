using BU.BOS.Collections.Generic;
using BU.BOS.Orm.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace BU.BOS.Orm.Metadata.DataEntity
{
    [Serializable]
    public sealed class DynamicPropertyCollection : KeyedCollectionBase<string, DynamicProperty>, IDataEntityPropertyCollection, IKeyedCollectionBase<string, IDataEntityProperty>, IEnumerable<IDataEntityProperty>, IEnumerable
    {
        #region Fields
        [NonSerialized]
        private DynamicObjectType _reflectedType;
        #endregion

        #region Properties
        protected override IEqualityComparer<string> Comparer
        {
            get
            {
                return StringComparer.OrdinalIgnoreCase;
            }
        }

        IDataEntityProperty IKeyedCollectionBase<string, IDataEntityProperty>.this[int index]
        {
            get
            {
                return base[index];
            }
        }

        IDataEntityProperty IKeyedCollectionBase<string, IDataEntityProperty>.this[string key]
        {
            get
            {
                return base[key];
            }
        }

        [ReadOnly(true)]
        public DynamicObjectType ReflectedType
        {
            get
            {
                return this._reflectedType;
            }
            internal set
            {
                this._reflectedType = value;
            }
        }
        #endregion

        #region Methods
        internal DynamicPropertyCollection(IList<DynamicProperty> list, DynamicObjectType reflectedType)
            : base(list)
        {
            this._reflectedType = reflectedType;
            this.ResetPropertyIndex();
        }

        internal void Add(DynamicProperty property)
        {
            lock (this)
            {
                DynamicProperty property2;
                if (base.TryGetValue(property.Name, out property2))
                {
                    throw new ORMDesignException("??????", string.Format("往属性集合{0}中添加属性{1}失败，已经存在此属性！", this.ReflectedType.Name, property.Name));
                }
                base.Items.Add(property);
                property.Ordinal = base.Count - 1;
                property.ReflectedType = this._reflectedType;
                base.Dictionary.Add(property.Name, property);
            }
        }

        internal void AddRange(DynamicProperty[] properties)
        {
            lock (this)
            {
                foreach (DynamicProperty property in properties)
                {
                    DynamicProperty property2;
                    if (base.TryGetValue(property.Name, out property2))
                    {
                        throw new ORMDesignException("??????", string.Format("往属性集合{0}中添加属性{1}失败，已经存在此属性！", this.ReflectedType.Name, property.Name));
                    }
                    base.Items.Add(property);
                    property.Ordinal = base.Count - 1;
                    property.ReflectedType = this._reflectedType;
                    base.Dictionary.Add(property.Name, property);
                }
            }
        }

        public bool Contains(string key)
        {
            DynamicProperty property;
            return base.TryGetValue(key, out property);
        }

        protected override string GetKeyForItem(DynamicProperty item)
        {
            if (item == null)
            {
                return null;
            }
            return item.Name;
        }

        bool IKeyedCollectionBase<string, IDataEntityProperty>.Contains(IDataEntityProperty item)
        {
            return base.Contains((DynamicProperty)item);
        }

        void IKeyedCollectionBase<string, IDataEntityProperty>.CopyTo(IDataEntityProperty[] array, int arrayIndex)
        {
            for (int i = 0; i < base.Count; i++)
            {
                array[i + arrayIndex] = base[i];
            }
        }

        int IKeyedCollectionBase<string, IDataEntityProperty>.IndexOf(IDataEntityProperty item)
        {
            return base.IndexOf((DynamicProperty)item);
        }

        IDataEntityProperty[] IKeyedCollectionBase<string, IDataEntityProperty>.ToArray()
        {
            IDataEntityProperty[] propertyArray = new IDataEntityProperty[base.Count];
            for (int i = 0; i < base.Count; i++)
            {
                propertyArray[i] = base[i];
            }
            return propertyArray;
        }

        bool IKeyedCollectionBase<string, IDataEntityProperty>.TryGetValue(string key, out IDataEntityProperty value)
        {
            DynamicProperty property;
            if (base.TryGetValue(key, out property))
            {
                value = property;
                return true;
            }
            value = null;
            return false;
        }

        private void ResetPropertyIndex()
        {
            for (int i = 0; i < base.Items.Count; i++)
            {
                DynamicProperty property = base.Items[i];
                if (property != null)
                {
                    property.Ordinal = i;
                    property.ReflectedType = this._reflectedType;
                }
            }
        }

        IEnumerator<IDataEntityProperty> IEnumerable<IDataEntityProperty>.GetEnumerator()
        {
            foreach (DynamicProperty iteratorVariable0 in this)
            {
                yield return iteratorVariable0;
            }
        }

        internal bool TryFind(DynamicProperty property, int lastIndex, out DynamicProperty find)
        {
            if (this.TryFindPrivate(property, lastIndex, out find))
            {
                return true;
            }
            int ordinal = property.Ordinal;
            return (((ordinal != lastIndex) && this.TryFindPrivate(property, ordinal, out find)) || base.TryGetValue(property.Name, out find));
        }

        private bool TryFindPrivate(DynamicProperty property, int predictedIndex, out DynamicProperty find)
        {
            if ((predictedIndex >= 0) && (predictedIndex < base.Count))
            {
                find = base[predictedIndex];
                if (object.Equals(find, property) || this.Comparer.Equals(find.Name, property.Name))
                {
                    return true;
                }
            }
            find = null;
            return false;
        }


        #endregion
    }
}
