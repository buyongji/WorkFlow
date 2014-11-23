using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BU.BOS
{
    [Serializable]
    public class Context : ISerializable, IDeserializationCallback, ICloneable
    {

        #region Field
        private OrganizationInfo _CurrentOrgInfo;
        private List<int> _sysLanguageIds;
        private List<LanguageInfo> _sysLanguages;
        private List<int> _useLanguageIds;
        private List<LanguageInfo> _useLanguages;
        private string AreaCacheKey;
        private bool bIsMultiOrg;
        public string CharacterSet;
        [NonSerialized]
        private CultureInfo ci;
        //private KDTimeZone currentUserTimeZone;
        private string dbId;
        public readonly CultureInfo DefaultLocale;
        [NonSerialized]
        private CultureInfo logLocale;
        private Context queryDBContext;



        //public readonly CultureInfo DefaultLocale;
        //public readonly string CharacterSet;

        #endregion

        #region Properties
        public string CallerName { get; set; }

        public string CallStack { get; set; }

        //public ClientType ClientType { get; set; }

        public string ComputerName { get; set; }

        public string ConsoleFormId { get; set; }

        public string ConsolePageId { get; set; }

        public string ContextId { get; private set; }

        public string CreateContextGuid { get; set; }

        public OrganizationInfo CurrentOrganizationInfo
        {
            get
            {
                return this._CurrentOrgInfo;
            }
            set
            {
                this._CurrentOrgInfo = value;
            }
        }

        public string CurrentServerName { get; set; }

        public Thread CurrentThread
        {
            get
            {
                return Thread.CurrentThread;
            }
        }

        //public KDTimeZone CurrentUserTimeZone
        //{
        //    get
        //    {
        //        return this.currentUserTimeZone;
        //    }
        //    set
        //    {
        //        this.currentUserTimeZone = value;
        //        if ((value == null) && (AppDomain.CurrentDomain.BaseDirectory.IndexOf("ManageSite") < 0))
        //        {
        //            Logger.Error("Context", "currentUserTimeZone==null", new Exception("value is null"));
        //        }
        //    }
        //}

        //public DataBaseCategory DatabaseCategory { get; set; }

        //public DatabaseType DatabaseType { get; set; }

        public string DataCenterName { get; set; }

        public string DBId
        {
            get
            {
                return this.dbId;
            }
            set
            {
                this.AreaCacheKey = string.Empty;
                this.dbId = value;
            }
        }

        public string IpAddress { get; set; }

        public bool IsCH_ZH_AutoTrans { get; set; }

        public bool IsMultiOrg
        {
            get
            {
                return this.bIsMultiOrg;
            }
            set
            {
                this.AreaCacheKey = string.Empty;
                this.bIsMultiOrg = value;
            }
        }

        public bool IsStartTimeZoneTransfer { get; set; }

        //public LightAppContext LightApp { get; set; }

        public string LoginName { get; set; }

        public CultureInfo LogLocale
        {
            get
            {
                return this.logLocale;
            }
            set
            {
                this.logLocale = value;
            }
        }

        public string ModuleName { get; set; }

        public string NetCtrlMonitorIDForDataCenterM { get; set; }

        public string QueryDBId { get; set; }

        //public Region Region { get; set; }

        public string Salt { get; set; }

        public string ServerUrl { get; set; }

        //public WebType ServiceType { get; set; }

        public string SessionId { get; set; }

        public List<int> SysLanguageIds
        {
            get
            {
                if ((this._sysLanguageIds == null) || (this._sysLanguageIds.Count == 0))
                {
                    this._sysLanguageIds = new List<int>();
                    foreach (LanguageInfo info in this.SysLanguages)
                    {
                        this._sysLanguageIds.Add(info.LocaleId);
                    }
                }
                return this._sysLanguageIds;
            }
        }

        public List<LanguageInfo> SysLanguages
        {
            get
            {
                if (this._sysLanguages == null)
                {
                    this._sysLanguages = new List<LanguageInfo>();
                }
                return this._sysLanguages;
            }
            set
            {
                this._sysLanguages = value;
                if (this._sysLanguages != null)
                {
                    this._sysLanguages.Clear();
                }
            }
        }

        //public KDTimeZone SystemTimeZone { get; set; }

        public string TenantId { get; set; }

        //public IsolationLevel TransIsolationLevel { get; set; }

        public List<int> UseLanguageIds
        {
            get
            {
                if ((this._useLanguageIds == null) || (this._useLanguageIds.Count == 0))
                {
                    this._useLanguageIds = new List<int>();
                    foreach (LanguageInfo info in this.UseLanguages)
                    {
                        this._useLanguageIds.Add(info.LocaleId);
                    }
                }
                return this._useLanguageIds;
            }
        }

        public List<LanguageInfo> UseLanguages
        {
            get
            {
                if (this._useLanguages == null)
                {
                    this._useLanguages = new List<LanguageInfo>();
                }
                return this._useLanguages;
            }
            set
            {
                this._useLanguages = value;
                if (this._useLanguageIds != null)
                {
                    this._useLanguageIds.Clear();
                }
            }
        }

        //public AuthenticationType UserAuthenticationMethod { get; set; }

        public string UserEmail { get; set; }

        public long UserId { get; set; }

        public CultureInfo UserLocale
        {
            get
            {
                return this.ci;
            }
            set
            {
                this.AreaCacheKey = string.Empty;
                this.ci = value;
            }
        }

        //public LoginType UserLoginType { get; set; }

        public string UserName { get; set; }

        public string UserPhone { get; set; }

        public string UserToken { get; set; }

        public string UserTransactionId { get; set; }

        //public X509Certificate UserX509Certificate { get; set; }

        //public KDOAuthInfo WeiboAuthInfo { get; set; }

        #endregion

        #region Methods
        public object Clone()
        {
            return base.MemberwiseClone();
        }
        public void GetObjectData(SerializationInfo info,StreamingContext context)
        {
            info.AddValue("UserLocale",this.UserLocale.Name);
            if(this.UserLocale!=null)
            {
                info.AddValue("LogLocale", this.LogLocale.Name);
            }
            else
            {
                info.AddValue("LogLocale","");
            }
            info.AddValue("DBid",this.DBId);
            //info.AddValue("DatabaseType", (int)this.DatabaseType);
            info.AddValue("IsMultiOrg", this.IsMultiOrg);
            info.AddValue("UseLanguages", this.UseLanguages);
            info.AddValue("UserId", this.UserId);
            info.AddValue("UserToken", this.UserToken);
            info.AddValue("UserName", this.UserName);
            info.AddValue("CurrentOrganizationInfo", this.CurrentOrganizationInfo);
            //info.AddValue("CurrentUserTimeZone", this.CurrentUserTimeZone);
            //info.AddValue("SystemTimeZone", this.SystemTimeZone);
            //info.AddValue("Region", this.Region);
            info.AddValue("ContextId", this.ContextId);
            info.AddValue("UserTransactionId", this.UserTransactionId);
            info.AddValue("IsCH_ZH_AutoTrans", this.IsCH_ZH_AutoTrans);
            //info.AddValue("WeiboAuthInfo", this.WeiboAuthInfo);
            info.AddValue("SessionId", this.SessionId);
            info.AddValue("ServerUrl", this.ServerUrl);
            info.AddValue("QueryDBId", this.QueryDBId);
            info.AddValue("TenantId", this.TenantId);
        }

        public void OnDeserialization(object sender)
        {
        }

        #endregion
    }
}
