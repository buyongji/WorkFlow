using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.ComponentModel;
using BU.BOS.Orm.Exceptions;
using BU.BOS.Orm.DataEntity;

namespace BU.BOS.Orm.Metadata.DataEntity
{
    [Serializable]
    public class DynamicObjectType : DynamicMetadata, IDataEntityType, IMetadata, ICustomAttributeProvider, IDeserializationCallback, ISerializable
    {
        #region Fields
        private object[] _attributes;
        private DynamicObjectType _baseType;
        [NonSerialized]
        private int? _cachedHashcode;
        private DataEntityCacheType _cacheType;
        private Type _clrType;
        [NonSerialized]
        private ICustomTypeDescriptor _customTypeDescriptor;
        private string _extendName;
        private DataEntityTypeFlag _flag;
        private List<DynamicObjectType> _interfaces;
        private string _name;
        private DynamicSimpleProperty _primaryKey;
        private DynamicPropertyCollection _properties;
        private SerializationInfo _serInfo;

        #endregion

        #region Properties
        public DynamicObjectType BaseType
        {
            get
            {
                return this._baseType;
            }
            private set
            {
                if (value != null)
                {
                    if (this.Flag == DataEntityTypeFlag.Interface)
                    {
                        if (value.Flag != DataEntityTypeFlag.Interface)
                        {
                            throw new ORMDesignException("??????", string.Format("设置动态实体类型{0}的基类失败,设置的值为接口,只能继承自接口！", this.Name));
                        }
                    }
                    else
                    {
                        if (value.Flag == DataEntityTypeFlag.Sealed)
                        {
                            throw new ORMDesignException("??????", string.Format("设置动态实体类型{0}的基类失败,设置的值封装类,不能继承！", this.Name));
                        }
                        if (value.Flag == DataEntityTypeFlag.Interface)
                        {
                            throw new ORMDesignException("??????", string.Format("设置动态实体类型{0}的基类失败,设置的值为接口,只能继承自接口！", this.Name));
                        }
                    }
                }
                for (DynamicObjectType type = value; type != null; type = type.BaseType)
                {
                    if (object.Equals(type, this))
                    {
                        throw new ORMDesignException("??????", string.Format("设置动态实体类型{0}的基类失败,不能循环继承！", this.Name));
                    }
                }
                //if (value != null)
                //{
                //    List<DynamicProperty> list = new List<DynamicProperty>(value._properties.Count);
                //    foreach (DynamicProperty property in value._properties)
                //    {
                //        list.Add(property.Clone());
                //    }
                //    this._properties = new DynamicPropertyCollection(list, this);
                //    if (value._primaryKey != null)
                //    {
                //        this._primaryKey = (DynamicSimpleProperty)this._properties[value._primaryKey.Name];
                //    }
                //}
                //else
                //{
                //    this._properties = new DynamicPropertyCollection(new List<DynamicProperty>(), this);
                //}
                this._baseType = value;
                //this._clrType = ReSetClrType(this);
            }
        }

        public DataEntityCacheType CacheType
        {
            get
            {
                return this._cacheType;
            }
            set
            {
                this._cacheType = value;
            }
        }

        public Type ClrType
        {
            get
            {
                if (this._clrType == null)
                {
                    return typeof(DynamicObject);
                }
                return this._clrType;
            }
            private set
            {
                if ((value != null) && !typeof(DynamicObject).IsAssignableFrom(value))
                {
                    throw new ORMArgInvalidException("??????", string.Format("设置动态实体类型{0}的类型Type失败,设置的值必须从DynamicObject派生！", this.Name));
                }
                if (value == typeof(DynamicObject))
                {
                    value = null;
                }
                this._clrType = value;
            }
        }

        internal ICustomTypeDescriptor CustomTypeDescriptor
        {
            get
            {
                if (this._customTypeDescriptor == null)
                {
                    this._customTypeDescriptor = new DynamicObjectTypeDescriptor(this);
                }
                return this._customTypeDescriptor;
            }
        }

        public string ExtendName
        {
            get
            {
                return this._extendName;
            }
            set
            {
                this._extendName = value;
            }
        }

        public DataEntityTypeFlag Flag
        {
            get
            {
                return this._flag;
            }
        }

        ISimpleProperty IDataEntityType.PrimaryKey
        {
            get
            {
                return this._primaryKey;
            }
        }

        IDataEntityPropertyCollection IDataEntityType.Properties
        {
            get
            {
                return this._properties;
            }
        }

        public override string Name
        {
            get
            {
                return this._name;
            }
        }

        public DynamicProperty PrimaryKey
        {
            get
            {
                return this._primaryKey;
            }
        }

        public DynamicPropertyCollection Properties
        {
            get
            {
                return this._properties;
            }
        }

        #endregion

        #region Methods
        private DynamicObjectType()
        {

        }

        protected DynamicObjectType(SerializationInfo info,StreamingContext context)
        {
            this._serInfo = info;
        }
        public DynamicObjectType(string name,DynamicObjectType baseType=null,Type dataEntityType=null,DataEntityTypeFlag flag=0,params object[] attributes)
        {
            this._flag = flag;
            this._baseType = baseType;

            this._attributes = attributes;
            this.ClrType = dataEntityType;
        }
        private void AddProperties(params DynamicProperty[] properties)
        {
            this._properties.AddRange(properties);
            this._cachedHashcode = null;
        }
        private static DynamicObject ConvertToDynamicObject(object dataEntity)
        {
            DynamicObject obj2 = dataEntity as DynamicObject;
            if (obj2 != null)
            {
                return obj2;
            }
            if (dataEntity == null)
            {
                throw new ORMArgInvalidException("??????","转换对象为动态实体失败，要转换的对象不能为空！");
            }
            throw new ORMArgInvalidException("??????", string.Format("转换对象{0}为动态实体失败，该对象必须是DynamicObject类型！", dataEntity.ToString()));
        }
        private int CreateHashCode()
        {
            int num = (((this._name == null) ? 0 : this._name.GetHashCode()) ^ this.ClrType.MetadataToken) ^ this._flag.GetHashCode();
            if (this._baseType != null)
            {
                num ^= this._baseType.GetHashCode();
            }
            if (this._primaryKey != null)
            {
                num ^= this._primaryKey.GetHashCode();
            }
            if (this._interfaces != null)
            {
                num ^= OrmUtils.GetListHashCode<DynamicObjectType>(this._interfaces);
            }
            return num;
        }



        #endregion

    }
}
