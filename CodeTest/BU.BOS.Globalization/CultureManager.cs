using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Globalization
{
    internal class CultureManager
    {
        #region Fields
        private static Context _userContext = null;
        private static Hashtable _htCulture = new Hashtable();
        private static string _logCultureInfoName = null;
        private static string _loginCultureInfName = "zh-CN";
        #endregion

        #region Properties
        internal static Context UserContext
        {
            get
            {
                return _userContext;
            }
            set
            {
                _userContext = value;
                if(_htCulture.ContainsKey(value.UserLocale.Name))
                {
                    _htCulture.Remove(value.UserLocale.Name);
                }
                _htCulture.Add(value.UserLocale.Name, value.UserLocale);
                if (_htCulture.ContainsKey(value.LogLocale.Name))
                {
                    _htCulture.Remove(value.LogLocale.Name);
                    _loginCultureInfName = value.LogLocale.Name;
                }
                _htCulture.Add(value.LogLocale.Name, value.LogLocale);

            }
        }
        #endregion

        #region Methods
        internal static void Clear()
        {
            if(_htCulture!=null)
            {
                _htCulture.Clear();
            }
            _htCulture = null;
        }

        private static string GetCurrentCultureName()
        {
            string name = null;
           
        }

      
        #endregion


    }
}
