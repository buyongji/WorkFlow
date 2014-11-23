using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.ComponentModel;

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
    }
}
