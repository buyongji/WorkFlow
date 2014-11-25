using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BU.BOS.Collections.Generic;
using System.Collections;

namespace BU.BOS.Orm.Metadata.DataEntity
{
    public interface IDataEntityPropertyCollection :IKeyedCollectionBase<string,IDataEntityProperty>,IEnumerable<IDataEntityProperty>,IEnumerable
    {
        #region Methods

        #endregion
    }
}