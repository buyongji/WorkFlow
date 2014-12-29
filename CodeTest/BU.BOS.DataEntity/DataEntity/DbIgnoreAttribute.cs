using System;
using System.Collections.Generic;
using System.Text;

namespace BU.BOS.Orm.DataEntity
{
    [Serializable, AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class DbIgnoreAttribute
    {
    }
}
