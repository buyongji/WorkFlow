using BU.BOS.Orm.Metadata.DataEntity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BU.BOS.Orm.DataEntity
{
    internal sealed class BoolDataEntityState : DataEntityState
    {
        #region Fields
        private BitArray _dirtyArray;
        private IDataEntityPropertyCollection _properties;
        private static readonly IEnumerable<IDataEntityProperty> EmptyDataEntityPropertyArray = new IDataEntityProperty[0];
        #endregion
     
        #region Properties
        public override bool DataEntityDirty
        {
            get
            {
                for (int i = 0; i < this._dirtyArray.Length; i++)
                {
                    if (this._dirtyArray[i])
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        #endregion

        #region Override
        public BoolDataEntityState(IDataEntityPropertyCollection properties, BitArray dirtyArray, PkSnapshot[] pkSnapshots, bool fromDatabase)
            : base(pkSnapshots, fromDatabase)
        {
            this._properties = properties;
            this._dirtyArray = dirtyArray;
        }

        public override IEnumerable<Metadata.DataEntity.IDataEntityProperty> GetDirtyProperties()
        {
            return this.GetDirtyProperties(false);
        }
        internal int[] GetDirtyFlags()
        {
            int length = this._dirtyArray.Length;
            if (length <= 0)
            {
                return null;
            }
            int num2 = ((length - 1) / 0x20) + 1;
            int[] array = new int[num2];
            this._dirtyArray.CopyTo(array, 0);
            return array;
        }
        public override IEnumerable<Metadata.DataEntity.IDataEntityProperty> GetDirtyProperties(bool includehasDefault)
        {
            List<IDataEntityProperty> list = new List<IDataEntityProperty>();
            IDataEntityProperty item = null;
            for (int i = 0; i < this._dirtyArray.Length; i++)
            {
                item = this._properties[i];
                if (this._dirtyArray[i] || (includehasDefault && item.HasDefaultValue))
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public override void SetDirty(bool newValue)
        {
            this._dirtyArray.SetAll(newValue);
        }

        public override void SetPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            DataEntityPropertyChangedEventArgs args = e as DataEntityPropertyChangedEventArgs;
            if (args == null)
            {
                IDataEntityProperty property;
                if (((e != null) && !string.IsNullOrEmpty(e.PropertyName)) && this._properties.TryGetValue(e.PropertyName, out property))
                {
                    this._dirtyArray[property.Ordinal] = true;
                }
            }
            else if (!args.IsErrorRaise)
            {
                this._dirtyArray[args.Property.Ordinal] = true;
            }
        }
        #endregion
    }
}
