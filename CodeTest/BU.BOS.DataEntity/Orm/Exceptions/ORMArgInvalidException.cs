using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Orm.Exceptions
{
    [Serializable]
  public  class ORMArgInvalidException:OrmException
  {
      #region Methods
      protected ORMArgInvalidException(SerializationInfo info, StreamingContext context)
          : base(info, context)
      {
      }

      public ORMArgInvalidException(string code, string message)
          : base(code, message)
      {
      }

      public ORMArgInvalidException(string code, string message, Exception inner)
          : base(code, message, inner)
      {
      }
      #endregion
  }
}
