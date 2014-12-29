using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Orm.Metadata.DataEntity
{
    public interface IDataEntityProperty : IMetadata, ICustomAttributeProvider
    {
        #region Methods
        object GetValue(object dataEntity);
        object GetValueFast(object dataEntity);
        void SetValue(object dataEntity, object value);
        void SetValueFast(object dataEntity, object value);
        bool HasDefaultValue{get;}
        bool IsReadOnly { get; }
        int Ordinal { get; }
        Type PropertyType { get; }
        #endregion
    }
}
