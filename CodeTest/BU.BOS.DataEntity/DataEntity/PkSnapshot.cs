using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BU.BOS.Orm.DataEntity
{
    [Serializable]
    public class PkSnapshot
    {
        #region Fields
        [DataMember(Name = "Oids", EmitDefaultValue = false)]
        internal string _oidsString;
        [DataMember(Name = "OidType", EmitDefaultValue = false)]
        internal string _oidType;
        private static readonly object[] EmptyObjectArray;
        public object[] Oids;
        [DataMember]
        public string TableName;
        #endregion

        #region Methods
        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            if (string.IsNullOrEmpty(this._oidType) || string.IsNullOrEmpty(this._oidsString))
            {
                this.Oids = EmptyObjectArray;
            }
            else
            {
                Type conversionType = Type.GetType("System." + this._oidType);
                List<object> list = new List<object>();
                foreach (string str in this._oidsString.Split(new char[] { ',' }))
                {
                    list.Add(Convert.ChangeType(str, conversionType, null));
                }
                this.Oids = list.ToArray();
            }
            this._oidType = null;
            this._oidsString = null;
        }

        [OnSerializing]
        internal void OnSerializing(StreamingContext context)
        {
            if ((this.Oids == null) || (this.Oids.Length == 0))
            {
                this._oidType = null;
                this._oidsString = null;
            }
            else if (this.Oids[0] != null)
            {
                this._oidType = this.Oids[0].GetType().Name;
                StringBuilder builder = new StringBuilder(this.Oids.Length * 4);
                for (int i = 0; i < this.Oids.Length; i++)
                {
                    if (i == 0)
                    {
                        builder.Append(this.Oids[i]);
                    }
                    else
                    {
                        builder.Append(",");
                        builder.Append(this.Oids[i]);
                    }
                }
                this._oidsString = builder.ToString();
            }
        }

        #endregion
    }
}
