using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Collections.Generic
{
    internal sealed class ForWriteListDebugView<T>
    {
        // Fields
        private ForWriteList<T> _list;

        // Methods
        public ForWriteListDebugView(ForWriteList<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            this._list = list;
        }

        // Properties
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                return this._list.ToArray();
            }
        }
    }
}
