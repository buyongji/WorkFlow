using BU.BOS.Orm.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BU.BOS.Orm.Metadata.DataEntity
{
    public abstract class DynamicPropertySetValueCallback
    {
        // Methods
        protected DynamicPropertySetValueCallback()
        {
        }

        protected IDataStorage GetDataStorage(DynamicObject obj)
        {
            return obj.DataStorage;
        }

        internal void SetNextCallback(DynamicPropertySetValueCallback nextCallback)
        {
            if (nextCallback == null)
            {
                this.NextHandler = null;
            }
            else
            {
                this.NextHandler = new SetValueCallbackDelegate(nextCallback.SetValue);
            }
        }

        public abstract void SetValue(DynamicObject obj, DynamicProperty property, object oldValue, ref object newValue);

        // Properties
        protected SetValueCallbackDelegate NextHandler { get; private set; }

        public abstract int Priority { get; }
    }

}
