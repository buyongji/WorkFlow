using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS
{
    public class ContextConverter : JsonConverter
    {
        #region Methods
        public ContextConverter()
        {

        }
        public override bool CanConvert(Type objectType)
        {
            return true;
        }
        #endregion
    }
}
