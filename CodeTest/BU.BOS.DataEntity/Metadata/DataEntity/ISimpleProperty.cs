using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Orm.Metadata.DataEntity
{
    public interface ISimpleProperty : IDataEntityProperty, IMetadata, ICustomAttributeProvider
    {
        #region Properties
        TypeConverter Converter { get; }
        bool IsLocalizable { get; }

        #endregion

        #region Methods
        void ResetValue(object dataEntity);
        bool ShouldSerializeValue(object dataEntity);
        #endregion
    }
}
