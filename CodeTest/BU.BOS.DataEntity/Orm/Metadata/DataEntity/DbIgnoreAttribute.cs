using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BU.BOS.Orm.Metadata.DataEntity
{
    [Serializable, AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class DbIgnoreAttribute : Attribute
    {
        public DbIgnoreAttribute()
        {

        }
    }
}
