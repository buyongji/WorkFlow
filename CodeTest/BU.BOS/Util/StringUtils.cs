using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Util
{
    public static class StringUtils
    {
        #region Methods
        public static bool IsEmpty(this string str)
        {
            return (str == null) || (str.Trim().Length <= 0);
        }
        #endregion
    }
}
