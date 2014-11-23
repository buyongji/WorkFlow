using BU.BOS.Orm.DataEntity;
using BU.BOS.Orm.Exceptions;
using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace BU.BOS.Orm.Metadata.DataEntity
{
    [Serializable]
    public class DynamicProperty : DynamicMetadata, IDataEntityProperty, IMetadata, ICustomAttributeProvider
    {
        #region Fields
        [DataMember(Name = "Attributes", EmitDefaultValue = false)]
        internal object[] _attributes;
        [DataMember(Name = "Hashcode", EmitDefaultValue = false)]
        internal int? _cachedHashcode;
        private static ConcurrentDictionary<TypeAndReadOnly, Tuple<GetValueCallbackDelegate, SetValueCallbackDelegate>> _defaultHandleCache = new ConcurrentDictionary<TypeAndReadOnly, Tuple<GetValueCallbackDelegate, SetValueCallbackDelegate>>();
        [DataMember(Name = "DefaultValue", EmitDefaultValue = false)]
        internal object _defaultValue;
        [NonSerialized]
        private GetValueCallbackDelegate _getValueHandle;
        [DataMember(Name = "HasDefaultValue", EmitDefaultValue = false)]
        internal bool _hasDefaultValue;
        [DataMember(Name = "IsReadonly", EmitDefaultValue = false)]
        internal readonly bool _isReadonly;
        [NonSerialized]
        private int _lastIndex;
        [DataMember(Name = "Name", IsRequired = true)]
        internal readonly string _name;
        [NonSerialized]
        private int _ordinal;
        [NonSerialized]
        private PropertyDescriptor _pdCache;
        [NonSerialized]
        private PropertyChangedEventArgs _propertyChangedEventArgsCache;
        [DataMember(Name = "PropertyType", EmitDefaultValue = false)]
        internal Type _propertyType;
        [NonSerialized]
        private DynamicObjectType _reflectedType;
        [NonSerialized]
        private SetValueCallbackDelegate _setValueHandle;
        #endregion

        #region Properties
        public object DefaultValue
        {
            get
            {
                return this._defaultValue;
            }
        }

        public bool HasDefaultValue
        {
            get
            {
                return this._hasDefaultValue;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this._isReadonly;
            }
        }

        public override string Name
        {
            get
            {
                return this._name;
            }
        }

        [ReadOnly(true)]
        public int Ordinal
        {
            get
            {
                return this._ordinal;
            }
            internal set
            {
                this._ordinal = value;
                this._lastIndex = value;
            }
        }

        internal PropertyChangedEventArgs PropertyChangedEventArgs
        {
            get
            {
                if (this._propertyChangedEventArgsCache == null)
                {
                    this._propertyChangedEventArgsCache = new DataEntityPropertyChangedEventArgs(this);
                }
                return this._propertyChangedEventArgsCache;
            }
        }

        internal PropertyDescriptor PropertyDescriptor
        {
            get
            {
                if (this._pdCache == null)
                {
                    this._pdCache = this.CreatePropertyDescriptor();
                }
                return this._pdCache;
            }
        }

        public Type PropertyType
        {
            get
            {
                return this._propertyType;
            }
        }

        [ReadOnly(true)]
        public DynamicObjectType ReflectedType
        {
            get
            {
                return this._reflectedType;
            }
            internal set
            {
                this._reflectedType = value;
            }
        }
        #endregion

        #region Methods
        protected DynamicProperty(DynamicProperty clone)
        {
            this._ordinal = -1;
            this._lastIndex = -1;
            this._name = clone._name;
            this._propertyType = clone._propertyType;
            this._defaultValue = clone._defaultValue;
            this._attributes = clone._attributes;
            this._isReadonly = clone._isReadonly;
            this.Initialize();
        }

        protected internal DynamicProperty(string name, Type propertyType, object defaultValue, object[] attributes, bool isReadonly)
        {
            this._ordinal = -1;
            this._lastIndex = -1;
            if (!IsValidIdentifier(name))
            {
                throw new ORMArgInvalidException("??????", "name:{0}不符合规范。");
            }
            if (propertyType == null)
            {
                throw new ORMArgInvalidException("??????", "propertyType不能为空");
            }
            this._name = name;
            this._propertyType = propertyType;
            this._defaultValue = defaultValue;
            this._attributes = attributes;
            this._isReadonly = isReadonly;
            this._hasDefaultValue = this.InnerHasDefaultValue();
            this.Initialize();
        }

        //private void BuildGetSetCallback(out GetValueCallbackDelegate getValueHandle, out SetValueCallbackDelegate setValueHandle)
        //{
        //    List<DynamicPropertyGetValueCallback> handlers = new List<DynamicPropertyGetValueCallback>(2);
        //    this.InitializeGetValueCallback(handlers);
        //    List<DynamicPropertySetValueCallback> list2 = new List<DynamicPropertySetValueCallback>(4);
        //    this.InitializeSetValueCallback(list2);
        //    object[] customAttributes = base.GetCustomAttributes(typeof(GetSetValueCallbackAttribute), true);
        //    if ((customAttributes != null) && (customAttributes.Length > 0))
        //    {
        //        foreach (GetSetValueCallbackAttribute attribute in customAttributes)
        //        {
        //            attribute.BuildGetCallbackHandlers(handlers);
        //            attribute.BuildSetCallbackHandlers(list2);
        //        }
        //    }
        //    if (handlers.Count == 0)
        //    {
        //        throw new ORMArgInvalidException("??????", string.Format(ResManager.LoadKDString("动态属性{0}初始化失败，错误的重载造成没有任何取值策略!", "014009000001724", SubSystemType.SL, new object[0]), this.Name));
        //    }
        //    handlers.Sort(new Comparison<DynamicPropertyGetValueCallback>(DynamicProperty.SortGetCallback));
        //    for (int i = 0; i < (handlers.Count - 1); i++)
        //    {
        //        handlers[i].SetNextCallback(handlers[i + 1]);
        //    }
        //    DynamicPropertyGetValueCallback local1 = handlers[0];
        //    getValueHandle = new GetValueCallbackDelegate(local1.GetValue);
        //    if (this._isReadonly)
        //    {
        //        setValueHandle = null;
        //    }
        //    else
        //    {
        //        if (list2.Count == 0)
        //        {
        //            throw new ORMArgInvalidException("??????", string.Format(ResManager.LoadKDString("动态属性{0}初始化失败，错误的重载造成没有任何设置值策略!", "014009000001725", SubSystemType.SL, new object[0]), this.Name));
        //        }
        //        list2.Sort(new Comparison<DynamicPropertySetValueCallback>(DynamicProperty.SortSetCallback));
        //        for (int j = 0; j < (list2.Count - 1); j++)
        //        {
        //            list2[j].SetNextCallback(list2[j + 1]);
        //        }
        //        DynamicPropertySetValueCallback local2 = list2[0];
        //        setValueHandle = new SetValueCallbackDelegate(local2.SetValue);
        //    }
        //}

        protected internal virtual DynamicProperty Clone()
        {
            return new DynamicProperty(this);
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
                throw new ORMArgInvalidException("??????", "转换对象为动态实体失败，要转换的对象不能为空！");
            }
            throw new ORMArgInvalidException("??????", "转换对象{0}为动态实体失败，该对象必须是DynamicObject类型！");
        }

        internal virtual int CreateHashCode()
        {
            return ((this._name.GetHashCode() ^ this._propertyType.MetadataToken) ^ this._ordinal);
        }

        //protected internal virtual PropertyDescriptor CreatePropertyDescriptor()
        //{
        //    return new DataEntityPropertyDescriptor(this);
        //}

        internal virtual bool Equals(DynamicProperty obj)
        {
            return (((obj._name == this._name) && (obj._propertyType == this._propertyType)) && (obj._ordinal == this._ordinal));
        }

        public override bool Equals(object obj)
        {
            bool flag = base.Equals(obj);
            if (flag)
            {
                return this.Equals((DynamicProperty)obj);
            }
            return flag;
        }

        //internal DynamicProperty FindTrueProperty(DynamicObject dataEntity)
        //{
        //    DynamicProperty property;
        //    if (dataEntity == null)
        //    {
        //        throw new ORMArgInvalidException("??????", string.Format(ResManager.LoadKDString("寻找实体上{0}对应的属性描述符失败，实体不能为空！", "014009000001726", SubSystemType.SL, new object[0]), this.Name));
        //    }
        //    DynamicObjectType dynamicObjectType = dataEntity.DynamicObjectType;
        //    if (object.ReferenceEquals(this._reflectedType, dynamicObjectType))
        //    {
        //        return this;
        //    }
        //    if (dynamicObjectType.Properties.TryFind(this, this._lastIndex, out property))
        //    {
        //        this._lastIndex = property._ordinal;
        //        return property;
        //    }
        //    StringBuilder builder = new StringBuilder();
        //    foreach (DynamicProperty property2 in dataEntity.DynamicObjectType.Properties)
        //    {
        //        builder.Append(property2.Name).Append(" ");
        //    }
        //    throw new ORMArgInvalidException("??????", string.Format(ResManager.LoadKDString("寻找实体上{0}对应的属性描述符失败，实体不存在此属性！", "002025030001447", SubSystemType.BOS, new object[0]), this.Name) + string.Format("[EntityType：{0} Propeyties:{1}]", dataEntity.DynamicObjectType.Name, builder.ToString()));
        //}

        //public override object[] GetCustomAttributes(bool inherit)
        //{
        //    if (this._attributes == null)
        //    {
        //        return DynamicMetadata.EmptyAttributes;
        //    }
        //    return this._attributes;
        //}

        //private void GetDefaultGetSetValueCallback(out GetValueCallbackDelegate getValueHandle, out SetValueCallbackDelegate setValueHandle)
        //{
        //    Func<TypeAndReadOnly, Tuple<GetValueCallbackDelegate, SetValueCallbackDelegate>> valueFactory = null;
        //    if (base.GetType() == typeof(DynamicSimpleProperty))
        //    {
        //        if (this._isReadonly)
        //        {
        //            getValueHandle = new GetValueCallbackDelegate(DefaultGetSetValue.GetValue);
        //            setValueHandle = null;
        //        }
        //        else
        //        {
        //            getValueHandle = new GetValueCallbackDelegate(DefaultGetSetValue.GetValue);
        //            setValueHandle = new SetValueCallbackDelegate(DefaultGetSetValue.SetValue);
        //        }
        //    }
        //    else
        //    {
        //        TypeAndReadOnly key = new TypeAndReadOnly(base.GetType(), this._isReadonly);
        //        if (valueFactory == null)
        //        {
        //            valueFactory = delegate(TypeAndReadOnly k)
        //            {
        //                GetValueCallbackDelegate delegate2;
        //                SetValueCallbackDelegate delegate3;
        //                this.BuildGetSetCallback(out delegate2, out delegate3);
        //                return new Tuple<GetValueCallbackDelegate, SetValueCallbackDelegate>(delegate2, delegate3);
        //            };
        //        }
        //        Tuple<GetValueCallbackDelegate, SetValueCallbackDelegate> orAdd = _defaultHandleCache.GetOrAdd(key, valueFactory);
        //        getValueHandle = orAdd.Item1;
        //        setValueHandle = orAdd.Item2;
        //    }
        //}

        public override int GetHashCode()
        {
            if (!this._cachedHashcode.HasValue)
            {
                this._cachedHashcode = new int?(this.CreateHashCode());
            }
            return this._cachedHashcode.Value;
        }

        //public object GetValue(DynamicObject dataEntity)
        //{
        //    DynamicProperty property = this.FindTrueProperty(dataEntity);
        //    return property._getValueHandle(dataEntity, property);
        //}

        //public T GetValue<T>(DynamicObject dataEntity)
        //{
        //    DynamicProperty property = this.FindTrueProperty(dataEntity);
        //    return (T)property._getValueHandle(dataEntity, property);
        //}

        public object GetValueFast(DynamicObject dataEntity)
        {
            return this._getValueHandle(dataEntity, this);
        }

        private void Initialize()
        {
            if (base.IsDefined(typeof(GetSetValueCallbackAttribute), true))
            {
                this.BuildGetSetCallback(out this._getValueHandle, out this._setValueHandle);
            }
            else
            {
                this.GetDefaultGetSetValueCallback(out this._getValueHandle, out this._setValueHandle);
                if (this._isReadonly)
                {
                    this._setValueHandle = null;
                }
            }
        }

        //protected virtual void InitializeGetValueCallback(IList<DynamicPropertyGetValueCallback> handlers)
        //{
        //    handlers.Add(new LocalValueGetValueCallback());
        //    handlers.Add(new DefaultValueGetValueCallback());
        //}

        //protected virtual void InitializeSetValueCallback(IList<DynamicPropertySetValueCallback> handlers)
        //{
        //    handlers.Add(new RaiseChangingEventSetValueCallback());
        //    handlers.Add(new ChangeTypeSetValueCallback());
        //    handlers.Add(new LocalValueSetValueCallback());
        //    handlers.Add(new RaiseChangedEventSetValueCallback());
        //}

        private bool InnerHasDefaultValue()
        {
            if (this._propertyType == typeof(int))
            {
                if (object.Equals(this._defaultValue, 0))
                {
                    return false;
                }
            }
            else if (this._propertyType == typeof(long))
            {
                if (object.Equals(this._defaultValue, 0L))
                {
                    return false;
                }
            }
            else if ((this._propertyType == typeof(decimal)) && object.Equals(this._defaultValue, 0M))
            {
                return false;
            }
            return (this._defaultValue != null);
        }

        private static bool IsValidIdentifier(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            return CodeGenerator.IsValidLanguageIndependentIdentifier(name);
        }

        object IDataEntityProperty.GetValue(object dataEntity)
        {
            DynamicObject obj2 = ConvertToDynamicObject(dataEntity);
            return this.GetValue(obj2);
        }

        object IDataEntityProperty.GetValueFast(object dataEntity)
        {
            DynamicObject obj2 = ConvertToDynamicObject(dataEntity);
            return this.GetValueFast(obj2);
        }

        //void IDataEntityProperty.SetValue(object dataEntity, object value)
        //{
        //    DynamicObject obj2 = ConvertToDynamicObject(dataEntity);
        //    this.SetValue(obj2, value);
        //}

        //void IDataEntityProperty.SetValueFast(object dataEntity, object value)
        //{
        //    DynamicObject obj2 = ConvertToDynamicObject(dataEntity);
        //    this.SetValueFast(obj2, value);
        //}

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            this.Initialize();
        }

        //public void ResetValue(DynamicObject dataEntity)
        //{
        //    this.FindTrueProperty(dataEntity).ResetValuePrivate(dataEntity);
        //}

        private void ResetValuePrivate(DynamicObject dataEntity)
        {
            if (this._isReadonly)
            {
                throw new ReadOnlyException();
            }
            object oldValue = this._getValueHandle(dataEntity, this);
            object newValue = this._defaultValue;
            this._setValueHandle(dataEntity, this, oldValue, ref newValue);
        }

        //public void SetValue(DynamicObject dataEntity, object newValue)
        //{
        //    this.FindTrueProperty(dataEntity).SetValuePrivate(dataEntity, newValue);
        //}

        //public void SetValueFast(DynamicObject dataEntity, object newValue)
        //{
        //    this.SetValuePrivate(dataEntity, newValue);
        //}

        //private void SetValuePrivate(DynamicObject dataEntity, object newValue)
        //{
        //    if (this._isReadonly)
        //    {
        //        throw new ReadOnlyException();
        //    }
        //    object oldValue = null;
        //    if (!dataEntity.Initializing)
        //    {
        //        oldValue = this._getValueHandle(dataEntity, this);
        //    }
        //    this._setValueHandle(dataEntity, this, oldValue, ref newValue);
        //}

        //private static int SortGetCallback(DynamicPropertyGetValueCallback a, DynamicPropertyGetValueCallback b)
        //{
        //    return a.Priority.CompareTo(b.Priority);
        //}

        //private static int SortSetCallback(DynamicPropertySetValueCallback a, DynamicPropertySetValueCallback b)
        //{
        //    return a.Priority.CompareTo(b.Priority);
        //}

        #endregion

        #region Delegates
        public delegate void SetValueCallbackDelegate(DynamicObject obj, DynamicProperty property, object oldValue, ref object newValue);
        public delegate object GetValueCallbackDelegate(DynamicObject obj, DynamicProperty property);
        #endregion

        #region Structs
        [StructLayout(LayoutKind.Sequential)]
        private struct TypeAndReadOnly
        {
            public Type Type;
            public bool IsReadonly;
            public TypeAndReadOnly(Type type, bool isReadonly)
            {
                this.Type = type;
                this.IsReadonly = isReadonly;
            }

            public override int GetHashCode()
            {
                return (this.Type.GetHashCode() ^ this.IsReadonly.GetHashCode());
            }

            public override bool Equals(object obj)
            {
                if (!(obj is DynamicProperty.TypeAndReadOnly))
                {
                    return false;
                }
                DynamicProperty.TypeAndReadOnly only = (DynamicProperty.TypeAndReadOnly)obj;
                return ((this.Type == only.Type) && (this.IsReadonly == only.IsReadonly));
            }
        }
        #endregion

    }
}
