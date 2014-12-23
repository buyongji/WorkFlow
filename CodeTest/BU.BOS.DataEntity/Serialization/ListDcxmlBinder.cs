using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BU.BOS.Serialization
{
    class ListDcxmlBinder
    {
        private IEnumerable<Orm.Metadata.DataEntity.IDataEntityType> dts;

        public ListDcxmlBinder(IEnumerable<Orm.Metadata.DataEntity.IDataEntityType> dts)
        {
            // TODO: Complete member initialization
            this.dts = dts;
        }
    }
}
