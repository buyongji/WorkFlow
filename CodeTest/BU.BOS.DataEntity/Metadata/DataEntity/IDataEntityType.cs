using BU.BOS.Orm.DataEntity;
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
        PkSnapshotSet GetPkSnapshot(object dataEntity);
        bool IsAssignableFrom(IDataEntityType dt);
        bool IsDirty(object dataEntity);
        void SetDirty(object dataEntity, bool newValue);
        void SetFromDatabase(object dataEntity);
        void SetPkSnapshot(object dataEntity, PkSnapshotSet pkSnapshotSet);

        // Properties
        DataEntityCacheType CacheType { get; }
        DataEntityTypeFlag Flag { get; }
        ISimpleProperty PrimaryKey { get; }
        IDataEntityPropertyCollection Properties { get; }
        
        #endregion
    }
}
