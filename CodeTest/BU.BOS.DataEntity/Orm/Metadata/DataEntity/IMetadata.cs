using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Orm.Metadata.DataEntity
{
    public interface IMetadata : ICustomAttributeProvider
    {
        string Name { get; }
    }
}
