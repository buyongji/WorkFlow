using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BU.BOS.Orm.Metadata.DataEntity;

namespace BU.BOS.Orm.DataEntity
{
  public  interface IDataStorage
  {
      #region Methods
      object GetLocalValue(DynamicProperty property);
      IDataStorage MemberClone();
      void SetLocalValue(DynamicProperty property,object value);
      #endregion
  }
}
