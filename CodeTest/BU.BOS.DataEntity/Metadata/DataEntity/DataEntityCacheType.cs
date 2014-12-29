using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BU.BOS.Orm.Metadata.DataEntity
{
   public enum DataEntityCacheType
    {
        [EnumMember(Value = "Multi")]
        Multi = 1,
        [EnumMember(Value = "Share")]
        Share = 0

    }
}
