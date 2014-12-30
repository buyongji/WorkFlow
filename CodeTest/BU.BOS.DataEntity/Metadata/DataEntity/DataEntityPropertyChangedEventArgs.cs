using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BU.BOS.Orm.Metadata.DataEntity
{
   public class DataEntityPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        // Fields
        private bool _isErrorRaise;
        private IDataEntityProperty _property;

        // Methods
        public DataEntityPropertyChangedEventArgs(IDataEntityProperty property)
            : base(property.Name)
        {
            this._property = property;
        }

        public DataEntityPropertyChangedEventArgs(IDataEntityProperty property, bool isErrorRaise)
            : base(property.Name)
        {
            this._property = property;
            this._isErrorRaise = isErrorRaise;
        }

        // Properties
        public bool IsErrorRaise
        {
            get
            {
                return this._isErrorRaise;
            }
        }

        public IDataEntityProperty Property
        {
            get
            {
                return this._property;
            }
        }

    }
}
