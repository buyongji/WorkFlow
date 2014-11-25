using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using BU.BOS.Orm.DataEntity;

namespace BU.BOS.Orm.Metadata.DataEntity
{
    [Serializable]
    public abstract class DynamicMetadata : IMetadata, ICustomAttributeProvider, IIsDefinedDbIgnoreAttribute
    {
        #region Fields
        [NonSerialized]
        private bool? _isDefinedDbIgnoreAttribute;
        public static object[] EmptyAttributes = new object[0];
        #endregion

        #region Properties
        public abstract string Name { get; }
        #endregion

        #region Methods
        protected DynamicMetadata()
        {

        }
        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                if (object.ReferenceEquals(obj, this))
                {
                    return true;
                }
                if (obj.GetType() == base.GetType())
                {
                    DynamicMetadata metadata = (DynamicMetadata)obj;
                    return (metadata.GetHashCode() == this.GetHashCode());
                }
            }
            return false;
        }
        public abstract object[] GetCustomAttributes(bool inherit);
        public object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            object[] customAttributes = this.GetCustomAttributes(inherit);
            if (customAttributes == null || customAttributes.Length <= 0)
            {
                return EmptyAttributes;
            }
            List<object> list = new List<object>(customAttributes.Length);
            foreach (object obj in customAttributes)
            {
                if (attributeType.IsInstanceOfType(obj))
                {
                    list.Add(obj);
                }
            }

            return list.ToArray();
        }
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
        public bool IsDefined(Type attributeType, bool inherit)
        {
            object[] customAttributes = this.GetCustomAttributes(inherit);
            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                foreach (object obj in customAttributes)
                {
                    if (attributeType.IsInstanceOfType(obj))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool IsDefinedDbIgnoreAttribute()
        {
            if (!this._isDefinedDbIgnoreAttribute.HasValue)
            {
                this._isDefinedDbIgnoreAttribute = new bool?(this.IsDefined(typeof(DbIgnoreAttribute), true));
            }
            return this._isDefinedDbIgnoreAttribute.Value;
        }

        #endregion
    }
}
