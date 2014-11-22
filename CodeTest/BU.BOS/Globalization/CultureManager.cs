using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
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

        internal static CultureInfo CurrentCulture
        {
            get
            {
                CultureInfo info = null;
                if(UserContext !=null)
                {
                    info = (CultureInfo)_htCulture[GetCurrentCultureName()];
                }
                if(info!=null)
                {
                    return info;
                }
                return new CultureInfo(GetCurrentCultureName());
            }
        }

        public static string LoginCultureInfoName
        {
            get
            {
                return _logCultureInfoName;
            }
            set
            {
                _logCultureInfoName = value;
            }
        }

        internal static CultureInfo CurrentLogCulture
        {
            get
            {
                if(_logCultureInfoName==null)
                {
                    lock(typeof(CultureManager))
                    {
                        if(_logCultureInfoName==null)
                        {
                            _logCultureInfoName = "zh-CN";
                        }
                    }
                }
                CultureInfo info = (CultureInfo)_htCulture[_logCultureInfoName];
                if(info!=null)
                {
                    return info;
                }
                return new CultureInfo(_logCultureInfoName);
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
            if(UserContext==null)
            {
                return LoginCultureInfoName;
            }
            name = UserContext.UserLocale.Name;
            if(name==null)
            {
                name = Thread.CurrentThread.CurrentCulture.Name;
            }
            return name;
        }

      
        #endregion


    }
}
