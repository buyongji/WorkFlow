using BU.BOS.Orm.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BU.BOS.Orm.Metadata.DataEntity
{
    public abstract class DynamicPropertyGetValueCallback
    {
        // Methods
        protected DynamicPropertyGetValueCallback()
        {
        }

        protected IDataStorage GetDataStorage(DynamicObject obj)
        {
            return obj.DataStorage;
        }

        public abstract object GetValue(DynamicObject obj, DynamicProperty property);
        internal void SetNextCallback(DynamicPropertyGetValueCallback nextCallback)
        {
            if (nextCallback == null)
            {
                this.NextHandler = null;
            }
            else
            {
                this.NextHandler = new GetValueCallbackDelegate(nextCallback.GetValue);
            }
        }

        // Properties
        protected GetValueCallbackDelegate NextHandler { get; private set; }

        public abstract int Priority { get; }
    }
}
