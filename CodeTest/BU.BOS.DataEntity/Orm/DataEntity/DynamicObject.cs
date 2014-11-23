using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;           
using System.ComponentModel;
using BU.BOS.Orm.Metadata.DataEntity;
using BU.BOS.Orm.Exceptions;

namespace BU.BOS.Orm.DataEntity
{
    [Serializable]
    public class DynamicObject:DataEntityBase,IDynamicMetaObjectProvider,ICustomTypeDescriptor
    {
        #region Fields
        private IDataStorage _dataStorage;
        private readonly DynamicObjectType _dt;
        #endregion

        #region Methods
        public DynamicObject(DynamicObjectType dt)
        {
            if(dt==null)
            {
                throw new ORMArgInvalidException("??????", "构造动态实体失败，构造参数：实体类型dt不能为空!");
            }
            if(dt)
        }
        #endregion
    }
}
