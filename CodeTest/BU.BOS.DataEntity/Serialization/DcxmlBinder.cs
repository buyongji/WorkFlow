using BU.BOS.Orm.DataEntity;
using BU.BOS.Orm.Metadata.DataEntity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace BU.BOS.Serialization
{
 public abstract   class DcxmlBinder
    {
        // Fields
        private CultureInfo _culture;
        private bool _onlyDbProperty = true;
        private static readonly Dictionary<string, Type> _primitiveTypes = new Dictionary<string, Type>(0x11, StringComparer.Ordinal);
        private StringComparison _stringComparison = StringComparison.OrdinalIgnoreCase;
        public const string ELEMENT = "Element";

        // Methods
        static DcxmlBinder()
        {
            _primitiveTypes.Add("boolean", typeof(bool));
            _primitiveTypes.Add("bool", typeof(bool));
            _primitiveTypes.Add("byte", typeof(byte));
            _primitiveTypes.Add("sByte", typeof(sbyte));
            _primitiveTypes.Add("int16", typeof(short));
            _primitiveTypes.Add("iint16", typeof(ushort));
            _primitiveTypes.Add("int32", typeof(int));
            _primitiveTypes.Add("int", typeof(int));
            _primitiveTypes.Add("uint32", typeof(uint));
            _primitiveTypes.Add("int64", typeof(long));
            _primitiveTypes.Add("uint64", typeof(ulong));
            _primitiveTypes.Add("intptr", typeof(IntPtr));
            _primitiveTypes.Add("uintptr", typeof(UIntPtr));
            _primitiveTypes.Add("char", typeof(char));
            _primitiveTypes.Add("double", typeof(double));
            _primitiveTypes.Add("single", typeof(float));
            _primitiveTypes.Add("string", typeof(string));
            _primitiveTypes.Add("decimal", typeof(decimal));
            _primitiveTypes.Add("guid", typeof(Guid));
            _primitiveTypes.Add("datetime", typeof(DateTime));
            _primitiveTypes.Add("timespan", typeof(TimeSpan));
            _primitiveTypes.Add("datetimeoffset", typeof(DateTimeOffset));
        }

        protected DcxmlBinder()
        {
        }

        public virtual Func<object, object, bool> BindEqualsFunc(Type dataType, Func<object, object, bool> defaultFunc)
        {
            return defaultFunc;
        }

        protected internal virtual Action<ISimpleProperty, XmlReader, object> BindReadAction(Type dataType, Action<ISimpleProperty, XmlReader, object> defaultAction)
        {
            return defaultAction;
        }

        public virtual string BindToName(IDataEntityType dt)
        {
            string name = dt.Name;
            if (name.EndsWith("Element", this._stringComparison))
            {
                return name.Substring(0, name.Length - 7);
            }
            return name;
        }

        public virtual Func<ISimpleProperty, object, object, string> BindToStringFunc(Type dataType, Func<ISimpleProperty, object, object, string> defaultFunc, out bool isCData)
        {
            isCData = false;
            return defaultFunc;
        }

        public virtual IDataEntityType BindToType(string elementName, IDictionary<string, string> attributes)
        {
            IDataEntityType type;
            if (this.TryBindToType(elementName, attributes, out type))
            {
                return type;
            }
            if (!elementName.EndsWith("Element", this._stringComparison))
            {
                string str = elementName + "Element";
                if (this.TryBindToType(str, attributes, out type))
                {
                    return type;
                }
            }
            if (elementName != null)
            {
                Type type2;
                string key = elementName.ToLowerInvariant();
                if (_primitiveTypes.TryGetValue(key, out type2))
                {
                    return type2.GetDataEntityType();
                }
            }
            return null;
        }

        public virtual object CreateInstance(IDataEntityType dt)
        {
            return dt.CreateInstance();
        }

        public virtual IDictionary<string, string> GetDataEntityAttributes(object dataEntity)
        {
            return null;
        }

        public virtual IDataEntityType GetDataEntityType(object dataEntity)
        {
            if (dataEntity == null)
            {
                return null;
            }
            IDataEntityBase base2 = dataEntity as IDataEntityBase;
            if (base2 != null)
            {
                return base2.GetDataEntityType();
            }
            Type type = dataEntity.GetType();
            if (!type.IsPrimitive && !(type == typeof(string)))
            {
                return null;
            }
            return type.GetDataEntityType();
        }

        public static Type GetPrimitiveType(string elementName)
        {
            if (elementName != null)
            {
                Type type;
                string key = elementName.ToLower();
                if (_primitiveTypes.TryGetValue(key, out type))
                {
                    return type;
                }
            }
            return null;
        }

        public virtual void ThrowException(SerializationException serializationException)
        {
            if (!serializationException.ExceptionData.CanIgnore)
            {
                throw serializationException;
            }
        }

        public abstract bool TryBindToType(string elementName, IDictionary<string, string> attributes, out IDataEntityType result);

        // Properties
        public CultureInfo Culture
        {
            get
            {
                if (this._culture == null)
                {
                    this._culture = CultureInfo.CurrentCulture;
                }
                return this._culture;
            }
            set
            {
                this._culture = value;
            }
        }

        public bool IgnoreCase
        {
            get
            {
                return (this._stringComparison == StringComparison.OrdinalIgnoreCase);
            }
            set
            {
                if (value)
                {
                    this._stringComparison = StringComparison.OrdinalIgnoreCase;
                }
                else
                {
                    this._stringComparison = StringComparison.Ordinal;
                }
            }
        }

        public bool OnlyDbProperty
        {
            get
            {
                return this._onlyDbProperty;
            }
            set
            {
                this._onlyDbProperty = value;
            }
        }

    }
}
