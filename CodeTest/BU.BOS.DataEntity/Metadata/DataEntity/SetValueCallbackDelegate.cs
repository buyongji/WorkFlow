using BU.BOS.Orm.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BU.BOS.Orm.Metadata.DataEntity
{
   public delegate void   SetValueCallbackDelegate(DynamicObject obj,DynamicProperty property,object oldValue,ref object newValue);
  
}
