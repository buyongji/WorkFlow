using System;
using System.Web;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Drawing;

namespace BU.BOS.Resource
{
    public static class ResManager
    {
        // Fields
        private static ImageDirectory enuImageSource = ImageDirectory.Gui;
        private static ResourceManager imageresourceManager;
        private static ResourceManager m_ResourceManager;
        private static string mClientPath;
        private static string mImageClientPath;
        private static ISLLanguageResourceProxy resourceProxy = null;
        private static string subSystemType;

        // Methods
        private static ResourceManager getimageResourceManager()
        {
            if (imageresourceManager == null)
            {
                string baseName = "Kingdee.K3.ImageRes";
                DirectoryInfo info = new DirectoryInfo(ImageClientPath);
                imageresourceManager = ResourceManager.CreateFileBasedResourceManager(baseName, info.FullName, null);
            }
            return imageresourceManager;
        }

        private static ResourceManager GetResManagerInstance(string language, string systemType)
        {
            if (HttpContext.Current != null)
            {
                object data = CallContext.GetData("KINGDEE_K3_RESMANAGER");
                if (data == null)
                {
                    data = LoadResourceAssembly(language, systemType);
                    CallContext.SetData("KINGDEE_K3_RESMANAGER", data);
                }
                return (ResourceManager)data;
            }
            if (m_ResourceManager == null)
            {
                m_ResourceManager = LoadResourceAssembly(language, systemType);
            }
            return m_ResourceManager;
        }

        public static Icon LoadIcon(string resourceID)
        {
            Icon icon;
            try
            {
                icon = (Icon)getimageResourceManager().GetObject(resourceID);
            }
            catch
            {
                return null;
            }
            return icon;
        }

        public static Image LoadImage(string resourceID)
        {
            Image image;
            try
            {
                image = (Image)getimageResourceManager().GetObject(resourceID);
            }
            catch
            {
                try
                {
                    image = (Image)getimageResourceManager().GetObject("imgTbtn_flower");
                }
                catch
                {
                    return null;
                }
            }
            return image;
        }

        public static string LoadKDString(string description, string resourceID)
        {
            if (TryGetSlKDString(resourceID, description, null, out description))
            {
                return description;
            }
            return LoadKDString(description, resourceID, subSystemType, new object[0]);
        }

        public static string LoadKDString(string description, string resourceID, SubSystemType systemType, params object[] args)
        {
            if (TryGetSlKDString(resourceID, description, systemType.ToString(), out description))
            {
                return description;
            }
            return LoadKDString(description, resourceID, CultureInfoUtils.GetCurrentCulture().Name, systemType.ToString(), args);
        }

        public static string LoadKDString(string description, string resourceID, string systemType, params object[] args)
        {
            if (TryGetSlKDString(resourceID, description, systemType, out description))
            {
                return description;
            }
            return LoadKDString(description, resourceID, CultureInfoUtils.GetCurrentCulture().Name, systemType, args);
        }

        private static string LoadKDString(string description, string resourceID, string language, string systemType, params object[] args)
        {
            string format = "";
            if (language == "zh-CN")
            {
                if ((args == null) || (args.Length <= 0))
                {
                    return description;
                }
                return string.Format(description, args);
            }
            if ((language == null) || (language.Trim() == ""))
            {
                language = CultureInfoUtils.GetOSSelectLanguageLocaleCode();
            }
            try
            {
                ResourceManager resManagerInstance = GetResManagerInstance(language, systemType);
                if (language.Equals(Thread.CurrentThread.CurrentUICulture.Name, StringComparison.InvariantCultureIgnoreCase) || (systemType.ToString() != resManagerInstance.BaseName))
                {
                    resManagerInstance = LoadResourceAssembly(language, systemType);
                    if (HttpContext.Current != null)
                    {
                        CallContext.SetData("KINGDEE_K3_RESMANAGER", resManagerInstance);
                    }
                }
                format = resManagerInstance.GetString(resourceID, new CultureInfo(language));
                if (((format != null) && (args != null)) && (args.Length > 0))
                {
                    format = string.Format(CultureInfo.CurrentCulture, format, args);
                }
            }
            catch
            {
            }
            if ((format == null) || (format.Length == 0))
            {
                format = (description.Trim().Length == 0) ? resourceID : description;
            }
            return format;
        }

        public static string LoadResFormat(string description, string resourceID, params object[] args)
        {
            return LoadResFormat(description, resourceID, subSystemType, args);
        }

        public static string LoadResFormat(string description, string resourceID, SubSystemType systemType, params object[] args)
        {
            return LoadResFormat(description, resourceID, systemType.ToString(), args);
        }

        public static string LoadResFormat(string description, string resourceID, string systemType, params object[] args)
        {
            string str = LoadKDString(description, resourceID, systemType, args);
            int length = args.Length;
            for (int i = 1; i <= length; i++)
            {
                str = str.Replace("%" + i, args[i - 1].ToString());
            }
            return str;
        }

        public static string LoadResFormat(string description, string resourceID, string sLanguage, SubSystemType systemType, params object[] args)
        {
            string str = LoadKDString(description, resourceID, sLanguage, systemType.ToString(), args);
            if (args != null)
            {
                int length = args.Length;
                for (int i = 1; i <= length; i++)
                {
                    str = str.Replace("%" + i, args[i - 1].ToString());
                }
            }
            return str;
        }

        private static ResourceManager LoadResourceAssembly(string language, string systemType)
        {
            ResourceManager manager;
            string type = systemType.ToString();
            if (type.IndexOf(',') > -1)
            {
                manager = new ResourceManager(TypesContainer.GetOrRegister(type));
            }
            else
            {
                manager = ResourceManager.CreateFileBasedResourceManager(type, ClientPath, null);
            }
            SubSystemTpe = systemType;
            return manager;
        }

        public static void SetSLLanguageResourceProxy(ISLLanguageResourceProxy proxy)
        {
            resourceProxy = proxy;
        }

        public static bool TryGetSlKDString(string resourceId, string description, string subSystemType, out string value)
        {
            value = description;
            if (!string.IsNullOrWhiteSpace(subSystemType) && (SubSystemType.SL.ToString() != subSystemType))
            {
                return false;
            }
            if (resourceProxy == null)
            {
                return false;
            }
            bool flag = resourceProxy.TryGetKDString(resourceId, out value);
            if (!flag)
            {
                value = description;
            }
            return flag;
        }

        // Properties
        public static string ClientPath
        {
            get
            {
                FileInfo info = null;
                if (mClientPath == null)
                {
                    string relativeSearchPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    if (relativeSearchPath == null)
                    {
                        info = new FileInfo(Assembly.GetExecutingAssembly().Location);
                        relativeSearchPath = info.DirectoryName;
                    }
                    mClientPath = relativeSearchPath;
                    if (!mClientPath.EndsWith(@"\"))
                    {
                        mClientPath = mClientPath + @"\";
                    }
                }
                return mClientPath;
            }
        }

        public static string ImageClientPath
        {
            get
            {
                FileInfo info = null;
                if (mImageClientPath == null)
                {
                    info = new FileInfo(Assembly.GetExecutingAssembly().Location);
                    mImageClientPath = info.DirectoryName;
                    if (!mImageClientPath.EndsWith(@"\"))
                    {
                        mImageClientPath = mImageClientPath + @"\";
                    }
                }
                return mImageClientPath;
            }
        }

        public static ImageDirectory ImageSource
        {
            get
            {
                return enuImageSource;
            }
            set
            {
                enuImageSource = value;
            }
        }

        public static string Language
        {
            get
            {
                return CultureInfoUtils.GetCurrentCulture().Name;
            }
        }

        public static string SubSystemTpe
        {
            get
            {
                if (((HttpContext.Current != null) && (HttpContext.Current.Session != null)) && (HttpContext.Current.Session["SubSystemType"] != null))
                {
                    subSystemType = HttpContext.Current.Session["SubSystemType"].ToString();
                    return subSystemType;
                }
                return subSystemType.ToString();
            }
            set
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
                {
                    HttpContext.Current.Session["SubSystemType"] = value;
                }
                else
                {
                    subSystemType = value;
                }
            }
        }

        // Nested Types
        public class CultureInfoUtils
        {
            // Methods
            public static CultureInfo GetCurrentCulture()
            {
                return Thread.CurrentThread.CurrentCulture;
            }

            public static string GetOSSelectLanguageLocaleCode()
            {
                CultureInfo info = new CultureInfo(GetSystemDefaultLangID());
                return info.Name;
            }

            public static int GetOSSelectLanguageLocaleID()
            {
                return GetSystemDefaultLangID();
            }

            [DllImport("kernel32.dll")]
            private static extern ushort GetSystemDefaultLangID();
        }

    }
}
