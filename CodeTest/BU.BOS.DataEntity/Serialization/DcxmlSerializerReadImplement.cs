using BU.BOS.Orm.DataEntity;
using BU.BOS.Orm.Metadata.DataEntity;
using BU.BOS.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace BU.BOS.Serialization
{
    internal sealed class DcxmlSerializerReadImplement
    {
        // Fields
        private DcxmlBinder _binder;
        private bool _colloctionIgnorePKValue;
        private DcxmlTypeDescriptorContext _context;
        private object _lastColEntity;
        private ICollectionProperty _lastColProperty;
        private IList _lastList;
        private XmlReader _reader;
        private Dictionary<Type, Action<ISimpleProperty, XmlReader, object>> _setValueActionsCache;
        private Stack<ISupportInitialize> _supportInitializeObjects;

        // Methods
        internal DcxmlSerializerReadImplement(DcxmlBinder binder, XmlReader reader)
        {
            if (binder == null)
            {
                throw new ArgumentNullException("binder");
            }
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            this._binder = binder;
            this._reader = reader;
            this._supportInitializeObjects = new Stack<ISupportInitialize>();
            this._context = new DcxmlTypeDescriptorContext(null);
            this._setValueActionsCache = new Dictionary<Type, Action<ISimpleProperty, XmlReader, object>>();
        }

        internal DcxmlSerializerReadImplement(DcxmlBinder binder, XmlReader reader, bool colloctionIgnorePKValue = false)
            : this(binder, reader)
        {
            this._colloctionIgnorePKValue = colloctionIgnorePKValue;
        }

        private IDataEntityType BindToType(IDataEntityType canUseType)
        {
            string name = this._reader.Name;
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            int attributeCount = this._reader.AttributeCount;
            for (int i = 0; i < attributeCount; i++)
            {
                this._reader.MoveToAttribute(i);
                attributes.Add(this._reader.Name, this._reader.Value);
            }
            IDataEntityType type = this._binder.BindToType(name, attributes);
            if ((type == null) && (canUseType != null))
            {
                string a = this._binder.BindToName(canUseType);
                if (this._binder.IgnoreCase ? string.Equals(a, name, StringComparison.OrdinalIgnoreCase) : string.Equals(a, name, StringComparison.Ordinal))
                {
                    type = canUseType;
                }
            }
            if (type == null)
            {
                SerializationException.SerializationExceptionData data = new SerializationException.SerializationExceptionData
                {
                    CanIgnore = false
                };
                this.ThrowXmlException("??????", string.Format(ResManager.LoadKDString("未能找到{0}对应的数据类型，请检查是否Xml中拼写错误。", "014009000001751", SubSystemType.SL, new object[0]), name), data, null);
            }
            return type;
        }

        private object ConvertFromString(ISimpleProperty property, object dataEntity, string str)
        {
            this._context.SetInstance(property, dataEntity);
            return property.Converter.ConvertFromString(this._context, this._binder.Culture, str);
        }

        private Action<ISimpleProperty, XmlReader, object> CreateSetValueAction(Type propertyType)
        {
            Action<ISimpleProperty, XmlReader, object> action;
            Action<ISimpleProperty, XmlReader, object> action2 = null;
            Action<ISimpleProperty, XmlReader, object> action3 = null;
            if (typeof(string).IsAssignableFrom(propertyType))
            {
                action = delegate(ISimpleProperty property, XmlReader reader, object entity)
                {
                    string str = reader.ReadString();
                    property.SetValue(entity, str);
                };
            }
            else if (typeof(ILocaleValue).IsAssignableFrom(propertyType))
            {
                if (action2 == null)
                {
                    action2 = delegate(ISimpleProperty property, XmlReader reader, object entity)
                    {
                        string str = reader.ReadString();
                        int lCID = this._binder.Culture.LCID;
                        if (this.ResetLoacaleValueBy2052 && (lCID == 0x804))
                        {
                            property.ResetValue(entity);
                        }
                        ILocaleValue value2 = (ILocaleValue)property.GetValue(entity);
                        if (value2 == null)
                        {
                            value2 = (ILocaleValue)this.ConvertFromString(property, entity, str);
                            property.SetValue(entity, value2);
                        }
                        if (str != string.Empty)
                        {
                            value2[lCID] = str;
                        }
                    };
                }
                action = action2;
            }
            else if (typeof(byte[]).IsAssignableFrom(propertyType))
            {
                action = delegate(ISimpleProperty property, XmlReader reader, object entity)
                {
                    byte[] buffer = new byte[0x200];
                    using (MemoryStream stream = new MemoryStream())
                    {
                        int num;
                        while ((num = reader.ReadElementContentAsBase64(buffer, 0, 0x200)) > 0)
                        {
                            stream.Write(buffer, 0, num);
                        }
                        property.SetValue(entity, stream.ToArray());
                    }
                };
            }
            else
            {
                if (action3 == null)
                {
                    action3 = delegate(ISimpleProperty property, XmlReader reader, object entity)
                    {
                        string str = reader.ReadString();
                        object obj2 = this.ConvertFromString(property, entity, str);
                        property.SetValue(entity, obj2);
                    };
                }
                action = action3;
            }
            action = this._binder.BindReadAction(propertyType, action);
            if (action == null)
            {
                throw new ArgumentNullException("BindReadDataAction");
            }
            return action;
        }

        private void DoCollectionPropertyAction(string action, ICollectionProperty property, object entity)
        {
            string str = action;
            if (str != null)
            {
                if (!(str == "add"))
                {
                    if (str == "edit")
                    {
                        object obj3;
                        int num2 = this.FindItemByOid(property, entity, out obj3);
                        if (num2 >= 0)
                        {
                            this.ReadElement(property.CollectionItemPropertyType, obj3);
                        }
                        else if (num2 == -1)
                        {
                            SerializationException.SerializationExceptionData data = new SerializationException.SerializationExceptionData
                            {
                                CanIgnore = true
                            };
                            this.ThrowXmlException("??????", ResManager.LoadKDString("试图编辑的节点在现有集合中没有找到。", "014009000001743", SubSystemType.SL, new object[0]), data, null);
                        }
                        goto Label_0196;
                    }
                    if (str == "remove")
                    {
                        if (!this.OnlyLocaleValue)
                        {
                            object obj4;
                            int index = this.FindItemByOid(property, entity, out obj4);
                            if (index >= 0)
                            {
                                this.SafeGetList(property, entity).RemoveAt(index);
                            }
                            else if (index == -1)
                            {
                                SerializationException.SerializationExceptionData data2 = new SerializationException.SerializationExceptionData
                                {
                                    CanIgnore = true
                                };
                                this.ThrowXmlException("??????", ResManager.LoadKDString("试图删除的节点在现有集合中没有找到。", "014009000001744", SubSystemType.SL, new object[0]), data2, null);
                            }
                        }
                        goto Label_0196;
                    }
                }
                else
                {
                    object obj2 = this.ReadElement(property.CollectionItemPropertyType, null);
                    IList list = this.SafeGetList(property, entity);
                    if (list != null)
                    {
                        if (this._colloctionIgnorePKValue)
                        {
                            list.Add(obj2);
                        }
                        else if (this.FindItemIndex(property, entity, obj2) < 0)
                        {
                            list.Add(obj2);
                        }
                    }
                    goto Label_0196;
                }
            }
            SerializationException.SerializationExceptionData data3 = new SerializationException.SerializationExceptionData
            {
                CanIgnore = true
            };
            this.ThrowXmlException("??????", string.Format(ResManager.LoadKDString("不能识别的集合操作符{0}", "014009000001745", SubSystemType.SL, new object[0]), action), data3, null);
        Label_0196:
            this.SafeDo();
        }

        private void DoComplexPropertyAction(string action, IComplexProperty property, object entity)
        {
            string str2 = action;
            if (str2 != null)
            {
                if (!(str2 == "edit"))
                {
                    if (str2 == "setnull")
                    {
                        property.SetValue(entity, null);
                        goto Label_00E4;
                    }
                }
                else
                {
                    string name = this._reader.Name;
                    bool isEmptyElement = false;
                    isEmptyElement = this._reader.IsEmptyElement;
                    if (!isEmptyElement)
                    {
                        isEmptyElement = !this.MoveToNextElement(name);
                    }
                    if (!isEmptyElement)
                    {
                        object obj2 = property.GetValue(entity);
                        object objB = this.ReadElement(property.ComplexPropertyType, obj2);
                        if (!property.IsReadOnly && !object.ReferenceEquals(obj2, objB))
                        {
                            property.SetValue(entity, objB);
                        }
                        this.MoveToNextElement(name);
                    }
                    else if (!property.IsReadOnly)
                    {
                        property.SetValue(entity, null);
                    }
                    goto Label_00E4;
                }
            }
            SerializationException.SerializationExceptionData data = new SerializationException.SerializationExceptionData
            {
                CanIgnore = true
            };
            this.ThrowXmlException("??????", string.Format(ResManager.LoadKDString("不能识别的属性操作符{0}", "014009000001742", SubSystemType.SL, new object[0]), action), data, null);
        Label_00E4:
            this.SafeDo();
        }

        private void DoSimplePropertyAction(string action, ISimpleProperty property, object entity)
        {
            string str = action;
            if (str != null)
            {
                if (!(str == "setvalue"))
                {
                    if (str == "reset")
                    {
                        if (typeof(ILocaleValue).IsAssignableFrom(property.PropertyType))
                        {
                            ILocaleValue value2 = (ILocaleValue)property.GetValue(entity);
                            if (value2 == null)
                            {
                                property.ResetValue(entity);
                            }
                            else if (property is SimpleProperty)
                            {
                                int lCID = this._binder.Culture.LCID;
                                if (lCID != 0x804)
                                {
                                    value2[lCID] = value2[0x804];
                                }
                                else
                                {
                                    property.ResetValue(entity);
                                }
                            }
                        }
                        else
                        {
                            property.ResetValue(entity);
                        }
                        goto Label_01FC;
                    }
                    if (str == "setnull")
                    {
                        try
                        {
                            property.SetValue(entity, null);
                        }
                        catch (Exception exception2)
                        {
                            SerializationException.SerializationExceptionData data2 = new SerializationException.SerializationExceptionData
                            {
                                CanIgnore = true
                            };
                            this.ThrowXmlException("??????", string.Format(ResManager.LoadKDString("在赋值{0}:{1}的值 Null 时失败，{2}", "014009000001741", SubSystemType.SL, new object[0]), property.Name, property.PropertyType.Name, exception2.Message), data2, exception2);
                        }
                        goto Label_01FC;
                    }
                }
                else
                {
                    try
                    {
                        this.GetSetValueAction(property.PropertyType)(property, this._reader, entity);
                    }
                    catch (Exception exception)
                    {
                        SerializationException.SerializationExceptionData data = new SerializationException.SerializationExceptionData
                        {
                            CanIgnore = true
                        };
                        this.ThrowXmlException("??????", string.Format(ResManager.LoadKDString("在赋值{0}:{1}的值'{2}'时失败，{3}", "014009000001740", SubSystemType.SL, new object[0]), new object[] { property.Name, property.PropertyType.Name, this._reader.ReadString(), exception.Message }), data, exception);
                    }
                    goto Label_01FC;
                }
            }
            SerializationException.SerializationExceptionData data3 = new SerializationException.SerializationExceptionData
            {
                CanIgnore = true
            };
            this.ThrowXmlException("??????", string.Format(ResManager.LoadKDString("不能识别的属性操作符{0}", "014009000001742", SubSystemType.SL, new object[0]), action), data3, null);
        Label_01FC:
            this.SafeDo();
        }

        internal void EndInitialize()
        {
            while (this._supportInitializeObjects.Count > 0)
            {
                this._supportInitializeObjects.Pop().EndInit();
            }
        }

        private int FindItemByOid(ICollectionProperty property, object entity, out object item)
        {
            string attribute = this._reader.GetAttribute("oid");
            return this.FindItemByOid(property, entity, attribute, out item);
        }

        private int FindItemByOid(ICollectionProperty property, object entity, string oid, out object item)
        {
            if (string.IsNullOrEmpty(oid))
            {
                SerializationException.SerializationExceptionData data = new SerializationException.SerializationExceptionData
                {
                    CanIgnore = true
                };
                this.ThrowXmlException("??????", ResManager.LoadKDString("试图处理节点却没有指定oid", "014009000001746", SubSystemType.SL, new object[0]), data, null);
                item = null;
                return -2;
            }
            ISimpleProperty primaryKey = property.CollectionItemPropertyType.PrimaryKey;
            if (primaryKey == null)
            {
                SerializationException.SerializationExceptionData data2 = new SerializationException.SerializationExceptionData
                {
                    CanIgnore = false
                };
                this.ThrowXmlException("??????", string.Format(ResManager.LoadKDString("需要处理的集合{0}其元素类型{1}没有定义主键。", "014009000001747", SubSystemType.SL, new object[0]), property.Name, property.CollectionItemPropertyType.Name), data2, null);
                item = null;
                return -2;
            }
            object objB = this.ConvertFromString(primaryKey, entity, oid);
            IList list = this.SafeGetList(property, entity);
            if (list == null)
            {
                item = null;
                return -2;
            }
            for (int i = 0; i < list.Count; i++)
            {
                item = list[i];
                if ((item != null) && object.Equals(this._binder.GetDataEntityType(item).PrimaryKey.GetValue(item), objB))
                {
                    return i;
                }
            }
            item = null;
            return -1;
        }

        private int FindItemIndex(ICollectionProperty property, object entity, object item)
        {
            object obj3;
            ISimpleProperty primaryKey = this._binder.GetDataEntityType(item).PrimaryKey;
            if (primaryKey == null)
            {
                return -1;
            }
            object obj2 = primaryKey.GetValue(item);
            string oid = (obj2 == null) ? "" : obj2.ToString();
            return this.FindItemByOid(property, entity, oid, out obj3);
        }

        private static string GetAttributeValue(XmlReader reader, string attName, string defaultValue)
        {
            string attribute = reader.GetAttribute(attName);
            if (string.IsNullOrEmpty(attribute))
            {
                return defaultValue;
            }
            return attribute.ToLower();
        }

        private IDataEntityType GetDataEntityType(object entity)
        {
            return this._binder.GetDataEntityType(entity);
        }

        private Action<ISimpleProperty, XmlReader, object> GetSetValueAction(Type propertyType)
        {
            Action<ISimpleProperty, XmlReader, object> action;
            if (!this._setValueActionsCache.TryGetValue(propertyType, out action))
            {
                action = this.CreateSetValueAction(propertyType);
                this._setValueActionsCache.Add(propertyType, action);
            }
            return action;
        }

        private static bool MoveToFirstElement(XmlReader reader)
        {
            while (!reader.EOF)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    return true;
                }
                reader.Read();
            }
            return false;
        }

        private bool MoveToNextElement(string currentName)
        {
            while (this._reader.Read())
            {
                XmlNodeType nodeType = this._reader.NodeType;
                if (nodeType != XmlNodeType.Element)
                {
                    if (nodeType == XmlNodeType.EndElement)
                    {
                        goto Label_001B;
                    }
                    continue;
                }
                return true;
            Label_001B:
                if (this._reader.Name == currentName)
                {
                    return false;
                }
                SerializationException.SerializationExceptionData data = new SerializationException.SerializationExceptionData
                {
                    CanIgnore = true
                };
                this.ThrowXmlException("??????", ResManager.LoadKDString("不应该出现其他节点的结束符号", "014009000001752", SubSystemType.SL, new object[0]), data, null);
            }
            return false;
        }

        private void Push(object entity)
        {
            ISupportInitialize item = entity as ISupportInitialize;
            if (item != null)
            {
                item.BeginInit();
                this._supportInitializeObjects.Push(item);
            }
        }

        private bool ReadCollectionProperty(ICollectionProperty property, object entity)
        {
            if (property == null)
            {
                return false;
            }
            string name = this._reader.Name;
            while (this.MoveToNextElement(name))
            {
                string action = GetAttributeValue(this._reader, "action", "add");
                this.DoCollectionPropertyAction(action, property, entity);
            }
            return true;
        }

        private bool ReadComplexProperty(IComplexProperty property, object entity)
        {
            if (property == null)
            {
                return false;
            }
            string action = GetAttributeValue(this._reader, "action", "edit");
            this.DoComplexPropertyAction(action, property, entity);
            return true;
        }

        public object ReadElement(IDataEntityType dt, object entity)
        {
            MoveToFirstElement(this._reader);
            string name = this._reader.Name;
            if (entity == null)
            {
                dt = this.BindToType(dt);
                if (dt.Flag != DataEntityTypeFlag.Primitive)
                {
                    entity = this._binder.CreateInstance(dt);
                }
            }
            else
            {
                IDataEntityType dataEntityType = this.GetDataEntityType(entity);
                if (dataEntityType != null)
                {
                    dt = this.BindToType(dt);
                    if ((!object.Equals(dt, dataEntityType) && !dt.IsAssignableFrom(dataEntityType)) && (dt.Flag != DataEntityTypeFlag.Primitive))
                    {
                        entity = this._binder.CreateInstance(dt);
                    }
                }
            }
            this.Push(entity);
            if (!this._reader.IsEmptyElement)
            {
                if (dt.Flag != DataEntityTypeFlag.Primitive)
                {
                    while (this.MoveToNextElement(name))
                    {
                        IDataEntityProperty property;
                        if ((dt.Properties.TryGetValue(this._reader.Name, out property) && !this.ReadSimpleProperty(property as ISimpleProperty, entity)) && (!this.ReadComplexProperty(property as IComplexProperty, entity) && !this.ReadCollectionProperty(property as ICollectionProperty, entity)))
                        {
                            SerializationException.SerializationExceptionData data = new SerializationException.SerializationExceptionData
                            {
                                CanIgnore = true
                            };
                            this.ThrowXmlException("??????", string.Format(ResManager.LoadKDString("XML节点中出现的属性{0}，必须是简单属性、复杂或集合属性的一种。", "014009000001739", SubSystemType.SL, new object[0]), property.Name), data, null);
                        }
                        this.SafeDo();
                    }
                    return entity;
                }
                string str2 = this._reader.ReadString();
                Type primitiveType = DcxmlBinder.GetPrimitiveType(dt.Name.ToLowerInvariant());
                if (primitiveType != typeof(string))
                {
                    entity = Convert.ChangeType(str2, primitiveType, this._binder.Culture);
                }
                else
                {
                    entity = str2;
                }
                this.SafeDo();
            }
            return entity;
        }

        private bool ReadSimpleProperty(ISimpleProperty property, object entity)
        {
            if (property == null)
            {
                return false;
            }
            string action = GetAttributeValue(this._reader, "action", "setvalue");
            this.DoSimplePropertyAction(action, property, entity);
            return true;
        }

        private void SafeDo()
        {
            this.SafeDo(null);
        }

        private void SafeDo(Action action)
        {
            string name = this._reader.Name;
            int depth = this._reader.Depth;
            if (!this._reader.IsEmptyElement)
            {
                do
                {
                    if (this._reader.Depth < depth)
                    {
                        return;
                    }
                    if (((this._reader.NodeType == XmlNodeType.EndElement) && (this._reader.Depth == depth)) && (this._reader.Name == name))
                    {
                        return;
                    }
                }
                while (this._reader.Read());
            }
        }

        private IList SafeGetList(ICollectionProperty property, object entity)
        {
            if (object.ReferenceEquals(this._lastColProperty, property) && object.ReferenceEquals(this._lastColEntity, entity))
            {
                return this._lastList;
            }
            object obj2 = property.GetValue(entity);
            if (obj2 == null)
            {
                if (property.IsReadOnly)
                {
                    SerializationException.SerializationExceptionData data = new SerializationException.SerializationExceptionData
                    {
                        CanIgnore = true
                    };
                    this.ThrowXmlException("??????", string.Format(ResManager.LoadKDString("集合属性{0}是只读且未初始化值，请初始化的值或提供Set功能。", "014009000001748", SubSystemType.SL, new object[0]), property.Name), data, null);
                }
                try
                {
                    obj2 = Activator.CreateInstance(property.PropertyType);
                    property.SetValue(entity, obj2);
                }
                catch (Exception exception)
                {
                    SerializationException.SerializationExceptionData data2 = new SerializationException.SerializationExceptionData
                    {
                        CanIgnore = true
                    };
                    this.ThrowXmlException("??????", string.Format(ResManager.LoadKDString("自动创建集合属性{0}的值失败，{1}。", "014009000001749", SubSystemType.SL, new object[0]), property.Name, exception.Message), data2, exception);
                }
            }
            IList list = obj2 as IList;
            if (list == null)
            {
                SerializationException.SerializationExceptionData data3 = new SerializationException.SerializationExceptionData
                {
                    CanIgnore = true
                };
                this.ThrowXmlException("??????", string.Format(ResManager.LoadKDString("集合属性{0}必须支持IList接口", "014009000001750", SubSystemType.SL, new object[0]), property.Name), data3, null);
            }
            this._lastColProperty = property;
            this._lastColEntity = entity;
            this._lastList = list;
            return list;
        }

        private void ThrowXmlException(string code, string message, SerializationException.SerializationExceptionData data, Exception innerException = null)
        {
            XmlTextReader reader = this._reader as XmlTextReader;
            if (reader != null)
            {
                data.LineNumber = reader.LineNumber;
                data.LinePosition = reader.LinePosition;
            }
            data.OnReading = true;
            this._binder.ThrowException(new SerializationException(code, message, data, innerException));
        }

        // Properties
        public bool OnlyLocaleValue { get; set; }

        public bool ResetLoacaleValueBy2052 { get; set; }
    }


}
