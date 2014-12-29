using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BU.BOS.Orm.Metadata.DataEntity
{
    [Serializable]
    internal sealed class DynamicSimpleProperty : DynamicProperty, ISimpleProperty, IDataEntityProperty, IMetadata, ICustomAttributeProvider
    {
        #region Fields
        [NonSerialized]
        private TypeConverter _converterCache;
        #endregion

        #region Methods
        internal DynamicSimpleProperty(DynamicSimpleProperty clone)
            : base(clone)
        {
        }

        #endregion
    }
}
