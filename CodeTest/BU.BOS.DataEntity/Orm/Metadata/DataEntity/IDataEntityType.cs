using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Orm.Metadata.DataEntity
{
    public interface IDataEntityType : IMetadata, ICustomAttributeProvider
    {
        #region Methods
        object CreateInstance();
        IEnumerable<IDataEntityProperty> GetDirtyProperties(object dataEntity);
        IEnumerable<IDataEntityProperty> GetDirtyProperties(object dataEntity, bool includeHasDefault);
        
        #endregion
    }
}
