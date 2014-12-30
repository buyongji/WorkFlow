using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace BU.BOS.Orm.Exceptions
{
    [Serializable]
    public class ORMDesignException : OrmException
    {
        #region Methods
        protected ORMDesignException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ORMDesignException(string code, string message)
            : base(code, message)
        {
        }

        public ORMDesignException(string code, string message, Exception inner)
            : base(code, message, inner)
        {
        }

        #endregion
    }
}
