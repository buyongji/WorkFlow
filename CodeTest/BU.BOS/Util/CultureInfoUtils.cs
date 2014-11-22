using BU.BOS.Globalization;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Util
{
    public class CultureInfoUtils
    {
        #region Fields
        private static ConcurrentDictionary<int, CultureInfo> _cachedCultureInfo;
        public static Hashtable htLangInfo;
        #endregion

        #region Methods
        static CultureInfoUtils()
        {
            _cachedCultureInfo = new ConcurrentDictionary<int, CultureInfo>();
            htLangInfo = new Hashtable();
        }

        public CultureInfoUtils()
        {

        }

        public static CultureInfo GetCurrentLogCulture()
        {
            return CultureManager.CurrentLogCulture;
        }

        #endregion

    }
}
