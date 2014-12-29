using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Orm.Metadata.DataEntity
{
    public enum DataEntityTypeFlag
    {
        [EnumMember(Value = "Abstract")]
        Abstract = 1,
        [EnumMember(Value = "Class")]
        Class = 0,
        [EnumMember(Value = "Interface")]
        Interface = 3,
        [EnumMember(Value = "Primitive")]
        Primitive = 4,
        [EnumMember(Value = "Sealed")]
        Sealed = 2
    }
}
