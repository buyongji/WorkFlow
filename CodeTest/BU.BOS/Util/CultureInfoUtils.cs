using BU.BOS.Globalization;
using BU.BOS.Resource;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BU.BOS.Util
{
    public class CultureInfoUtils
    {
        #region Fields
        private static ConcurrentDictionary<int, CultureInfo> _cachedCultureInfo;
        public static Hashtable htLangInfo;
        #endregion

        #region Properties
        private static CultureInfo CurrentCulture
        {
            get
            {
                return CultureManager.CurrentCulture;
            }
        }

        public static CultureInfo InvariantCulture
        {
            get
            {
                return CultureInfo.InvariantCulture;
            }
        }
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

        public static string FormatNameDetailInfo(string strHonorific, string strFirstName, string strLastName, string strTitle)
        {
            return FormatNameDetailInfo(CultureManager.CurrentCulture, strHonorific, strFirstName, strLastName, strTitle);
        }
        public static string FormatNameDetailInfo(CultureInfo cultureInfo, string strHonorific, string strFirstName, string strLastName, string strTitle)
        {
            switch (cultureInfo.Name)
            {
                case "zh-CN":
                    return string.Format("{0}{1}{2}{3}", new object[] { strHonorific, strLastName, strFirstName, strTitle });
                case "en-US":
                    return string.Format("{0}{1}{2}", strHonorific, strTitle, strLastName);
            }
            return string.Format("{0}{1}{2}", strHonorific, strTitle, strLastName);
        }
        public static CultureInfo GetCurrentCulture()
        {
            return Thread.CurrentThread.CurrentCulture;
        }
        public static CultureInfo GetCurrentLogCulture()
        {
            return CultureManager.CurrentLogCulture;
        }
        public static Hashtable GetInstallLangInfoSet()
        {
            if (htLangInfo.Count == 0)
            {
                List<LanguageInfo> list = new List<LanguageInfo> {
            new LanguageInfo(0x409, ResManager.LoadKDString("英文", "002016030001210", SubSystemType.BOS, new object[0])),
            new LanguageInfo(0x804, ResManager.LoadKDString("简体中文", "002016030001000", SubSystemType.BOS, new object[0])),
            new LanguageInfo(0xc04, ResManager.LoadKDString("繁体中文", "002016030001213", SubSystemType.BOS, new object[0]))
        };
                htLangInfo.Add(0x804, list);
                list = new List<LanguageInfo> {
            new LanguageInfo(0x409, "English"),
            new LanguageInfo(0x804, "Simplified Chinese"),
            new LanguageInfo(0xc04, "Traditional Chinese")
        };
                htLangInfo.Add(0x409, list);
                list = new List<LanguageInfo> {
            new LanguageInfo(0x409, ResManager.LoadKDString("英文", "002016030001210", SubSystemType.BOS, new object[0])),
            new LanguageInfo(0x804, ResManager.LoadKDString("簡體中文", "002016030001216", SubSystemType.BOS, new object[0])),
            new LanguageInfo(0xc04, ResManager.LoadKDString("繁體中文", "002016030001003", SubSystemType.BOS, new object[0]))
        };
                htLangInfo.Add(0xc04, list);
            }
            return htLangInfo;
        }
        /// <summary>
        /// Overloaded.  
        /// </summary>
        /// <param name="lcid"></param>
        /// <returns></returns>
        public static string GetLanguageNameAlias(int lcid)
        {
            CultureInfo info = new CultureInfo(lcid);
            return info.Name.Substring(info.Name.Length - 2);
        }
        public static string GetLanguageNativeName(int lcid)
        {
            CultureInfo info = new CultureInfo(lcid);
            return info.NativeName;
        }
        public static string GetLanguageNativeName(string name)
        {
            CultureInfo info = new CultureInfo(name);
            return info.NativeName;
        }
        //public static 

 

 


 

 

 


        #endregion

    }
}
