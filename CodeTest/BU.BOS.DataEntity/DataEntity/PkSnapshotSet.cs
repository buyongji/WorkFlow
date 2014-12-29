using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Orm.DataEntity
{
   [Serializable]
  public  class PkSnapshotSet
  {
      #region Fields
       public List<PkSnapshot> Snapshots;
      #endregion

      #region Methods
      public PkSnapshotSet(int capacity=0)
       {
           this.Snapshots = new List<PkSnapshot>(capacity);
       }
      #endregion
  }
}
