using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.ComponentModel;
using BU.BOS.Orm.Metadata.DataEntity;
using BU.BOS.Orm.Exceptions;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Diagnostics;

namespace BU.BOS.Orm.DataEntity
{
    [Serializable]
    public class DynamicObject : DataEntityBase, IDynamicMetaObjectProvider, ICustomTypeDescriptor
    {
        #region Fields
        private IDataStorage _dataStorage;
        private readonly DynamicObjectType _dt;
        #endregion

        #region Properties
        protected internal IDataStorage DataStorage
        {
            get
            {
                return this._dataStorage;
            }
            internal set
            {
                this._dataStorage = value;
            }
        }

        [DebuggerHidden]
        public DynamicObjectType DynamicObjectType
        {
            get
            {
                return this._dt;
            }
        }

        public object this[string propertyName]
        {
            get
            {
                DynamicProperty property;
                if (!this._dt.Properties.TryGetValue(propertyName, out property))
                {
                    throw new ORMDesignException("??????", string.Format("实体类型{0}中不存在名为{1}的属性", this._dt.Name, propertyName));
                }
                return property.GetValueFast(this);
            }
            set
            {
                DynamicProperty property;
                if (!this._dt.Properties.TryGetValue(propertyName, out property))
                {
                    throw new ORMDesignException("??????", string.Format("实体类型{0}中不存在名为{1}的属性", this._dt.Name, propertyName));
                }
                property.SetValue(this, value);
            }
        }

        public object this[int index]
        {
            get
            {
                DynamicProperty property = this._dt.Properties[index];
                return property.GetValueFast(this);
            }
            set
            {
                this._dt.Properties[index].SetValue(this, value);
            }
        }

        public object this[DynamicProperty property]
        {
            get
            {
                if (property == null)
                {
                    throw new ArgumentNullException("property");
                }
                return property.GetValueFast(this);
            }
            set
            {
                if (property == null)
                {
                    throw new ArgumentNullException("property");
                }
                property.SetValue(this, value);
            }
        }

        #endregion

        #region Methods
        public DynamicObject(DynamicObjectType dt)
        {
            if(dt==null)
            {
                throw new ORMArgInvalidException("??????", "构造动态实体失败，构造参数：实体类型dt不能为空!");
            }
            if(dt.Flag== DataEntityTypeFlag.Abstract)
            {
                throw new ORMArgInvalidException("??????", string.Format("构造动态实体失败，{0}为抽象类型，不允许被实例化！", dt.Name));
            }
            if (dt.Flag == DataEntityTypeFlag.Interface)
            {
                throw new ORMArgInvalidException("??????", string.Format("构造动态实体失败，{0}为接口类型，不允许被实例化！", dt.Name));
            }
            this._dt = dt;
            this._dataStorage = this.CreateDataStorage();
        }

        private IDataStorage CreateDataStorage()
        {
            if (this._dt.Properties.Count > 200)
            {
                return new DictionaryDataStorage(this._dt);
            }
            return new ArrayStorage(this._dt);
        }

        public override IDataEntityType GetDataEntityType()
        {
            return this._dt;
        }

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return this._dt.CustomTypeDescriptor.GetAttributes();
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return this._dt.CustomTypeDescriptor.GetClassName();
        }
        string ICustomTypeDescriptor.GetComponentName()
        {
            return this._dt.CustomTypeDescriptor.GetComponentName();
        }
        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return this._dt.CustomTypeDescriptor.GetConverter();
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return this._dt.CustomTypeDescriptor.GetDefaultEvent();
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return this._dt.CustomTypeDescriptor.GetDefaultProperty();
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return this._dt.CustomTypeDescriptor.GetEditor(editorBaseType);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return this._dt.CustomTypeDescriptor.GetEvents();
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return this._dt.CustomTypeDescriptor.GetEvents(attributes);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return this._dt.CustomTypeDescriptor.GetProperties();
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return this._dt.CustomTypeDescriptor.GetProperties(attributes);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            return new MetaDynamic(parameter, this);
        }

        private bool TryGetMember(GetMemberBinder binder, out object result)
        {
            DynamicProperty property;
            if (this._dt.Properties.TryGetValue(binder.Name, out property))
            {
                result = property.GetValueFast(this);
                return true;
            }
            result = null;
            return false;
        }

        private bool TrySetMember(SetMemberBinder binder, object dataEntity)
        {
            DynamicProperty property;
            if (this._dt.Properties.TryGetValue(binder.Name, out property))
            {
                property.SetValueFast(this, dataEntity);
                return true;
            }
            return false;
        }


        #endregion

        #region Structs
        [Serializable, StructLayout(LayoutKind.Sequential)]
        internal struct ArrayStorage : IDataStorage
        {
            private object[] _values;
            private ArrayStorage(object[] values)
            {
                this._values = values;
            }

            public ArrayStorage(DynamicObjectType dt)
            {
                this._values = new object[dt.Properties.Count];
            }

            public object GetLocalValue(DynamicProperty property)
            {
                int ordinal = property.Ordinal;
                if (ordinal >= this._values.Length)
                {
                    this.EnsureCapacity(property);
                }
                return this._values[ordinal];
            }

            private void EnsureCapacity(DynamicProperty property)
            {
                int newSize = (property.ReflectedType == null) ? property.Ordinal : property.ReflectedType.Properties.Count;
                Array.Resize<object>(ref this._values, newSize);
            }

            public void SetLocalValue(DynamicProperty property, object value)
            {
                int ordinal = property.Ordinal;
                if (ordinal >= this._values.Length)
                {
                    this.EnsureCapacity(property);
                }
                this._values[ordinal] = value;
            }

            public IDataStorage MemberClone()
            {
                object[] array = new object[this._values.Length];
                this._values.CopyTo(array, 0);
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] is ILocaleValue)
                    {
                        array[i] = null;
                    }
                }
                return new DynamicObject.ArrayStorage(array);
            }
        }


        [Serializable, StructLayout(LayoutKind.Sequential)]
        internal struct DictionaryDataStorage : IDataStorage
        {
            private ConcurrentDictionary<DynamicProperty, object> _values;
            private DictionaryDataStorage(ConcurrentDictionary<DynamicProperty, object> values)
            {
                this._values = values;
            }

            public DictionaryDataStorage(DynamicObjectType dt)
            {
                this._values = new ConcurrentDictionary<DynamicProperty, object>();
            }

            public object GetLocalValue(DynamicProperty property)
            {
                object obj2;
                this._values.TryGetValue(property, out obj2);
                return obj2;
            }

            public void SetLocalValue(DynamicProperty property, object value)
            {
                this._values[property] = value;
            }

            public IDataStorage MemberClone()
            {
                ConcurrentDictionary<DynamicProperty, object> values = new ConcurrentDictionary<DynamicProperty, object>();
                foreach (KeyValuePair<DynamicProperty, object> pair in this._values)
                {
                    if (pair.Value is ILocaleValue)
                    {
                        values[pair.Key] = null;
                    }
                    else
                    {
                        values[pair.Key] = pair.Value;
                    }
                }
                return new DynamicObject.DictionaryDataStorage(values);
            }
        }


        #endregion
    }
}
