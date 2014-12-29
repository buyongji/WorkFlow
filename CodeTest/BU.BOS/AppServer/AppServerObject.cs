using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.AppServer
{
    public abstract class AppServerObject
    {
        public AppServerObject(Context ctx)
        {
            this.Context = ctx;
        }
        protected Context Context { get; private set; }
    }
}
