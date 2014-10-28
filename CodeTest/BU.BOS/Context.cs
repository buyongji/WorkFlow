using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS
{
    public sealed class Context
    {

        public readonly CultureInfo DefaultLocale;
        public readonly string CharacterSet;
        public string ComputerName
        {
            get;
            set;
        }


        public object Clone();
        public Context GetQueryDBContext();
        public void OnDeserialization();
      
    }
}
