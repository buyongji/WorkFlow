using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BU.BOS.Orm.Metadata.DataEntity
{
    internal struct DynamicObjectTypeDescriptor
    {
        private readonly ICustomTypeDescriptor _parent;
        private readonly DynamicObjectType _dt;
        private readonly PropertyDescriptorCollection _properties;
        private readonly AttributeCollection _atts;
        public DynamicObjectTypeDescriptor(DynamicObjectType dt)
            : this(null, dt)
        {
        }

        public DynamicObjectTypeDescriptor(ICustomTypeDescriptor parent, DynamicObjectType dt)
        {
            this._parent = parent;
            this._dt = dt;
            this._atts = CreateAttributes(dt);
            this._properties = CreatePropertyDescriptorCollection(dt);
        }
         private static AttributeCollection CreateAttributes(DynamicObjectType dt)
    {
        object[] customAttributes = dt.GetCustomAttributes(true);
        if ((customAttributes == null) || (customAttributes.Length <= 0))
        {
            return AttributeCollection.Empty;
        }
        Attribute[] attributes = new Attribute[customAttributes.Length];
        for (int i = 0; i < customAttributes.Length; i++)
        {
            attributes[i] = (Attribute) customAttributes[i];
        }
        return new AttributeCollection(attributes);
    }

    private static PropertyDescriptorCollection CreatePropertyDescriptorCollection(DynamicObjectType dt)
    {
        DynamicPropertyCollection properties = dt.Properties;
        PropertyDescriptor[] descriptorArray = new PropertyDescriptor[properties.Count];
        for (int i = 0; i < properties.Count; i++)
        {
            descriptorArray[i] = properties[i].PropertyDescriptor;
        }
        return new PropertyDescriptorCollection(descriptorArray);
    }

    public AttributeCollection GetAttributes()
    {
        return this._atts;
    }

    public string GetClassName()
    {
        return this._dt.Name;
    }

    public string GetComponentName()
    {
        return this._dt.Name;
    }

    public TypeConverter GetConverter()
    {
        if (this._parent != null)
        {
            return this._parent.GetConverter();
        }
        return new TypeConverter();
    }

    public EventDescriptor GetDefaultEvent()
    {
        if (this._parent != null)
        {
            return this._parent.GetDefaultEvent();
        }
        return null;
    }

    public PropertyDescriptor GetDefaultProperty()
    {
        PropertyDescriptor descriptor = this._properties.Find("Name", true);
        if (descriptor != null)
        {
            return descriptor;
        }
        if (this._parent != null)
        {
            return this._parent.GetDefaultProperty();
        }
        return null;
    }

    public object GetEditor(Type editorBaseType)
    {
        if (this._parent != null)
        {
            return this._parent.GetEditor(editorBaseType);
        }
        return null;
    }

    public EventDescriptorCollection GetEvents(Attribute[] attributes)
    {
        if (this._parent != null)
        {
            return this._parent.GetEvents(attributes);
        }
        return EventDescriptorCollection.Empty;
    }

    public EventDescriptorCollection GetEvents()
    {
        if (this._parent != null)
        {
            return this._parent.GetEvents();
        }
        return EventDescriptorCollection.Empty;
    }

    public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
    {
        return this._properties;
    }

    public PropertyDescriptorCollection GetProperties()
    {
        return this._properties;
    }

    public object GetPropertyOwner(PropertyDescriptor pd)
    {
        return this._dt;


    }
}
