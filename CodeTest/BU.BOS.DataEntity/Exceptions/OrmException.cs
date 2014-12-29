using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Orm.Exceptions
{
    [Serializable]
    public class OrmException : ApplicationException
    {
        #region Fields
        private string _mCode;
        #endregion

        #region Properties
        public string Code
        {
            get
            {
                return this._mCode;
            }
        }
        #endregion

        #region Methods
        protected OrmException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this._mCode = info.GetString("Code");
        }
        public OrmException(string code, string message)
            : base(message)
        {
            this._mCode = code;
        }

        public OrmException(string code, string message, Exception inner)
            : base(message, inner)
        {
            this._mCode = code;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Code", this._mCode, typeof(string));
        }
        #endregion
    }
}
