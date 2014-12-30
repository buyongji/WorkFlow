using System;
using System.Collections.Generic;
using BU.BOS.Orm.DataEntity;
using System.Linq;
using System.Text;
using BU.BOS.Globalization;

namespace BU.BOS
{
    [Serializable]
    public class KDTimeZone
    {
        #region Fields
        /// <summary>
        /// 已审核状态
        /// </summary>
        const string AuditStatus = "C";
        /// <summary>
        /// 非禁用状态
        /// </summary>
        const string UnForbidStatus = "A";
        public static DateTime MinSystemDateTime = new DateTime(1900, 1, 1);
        public static DateTime MaxSystemDateTime = new DateTime(9999, 12, 31);
        #endregion
        #region Properties
        public long Id { get; private set; }
        public string Number { get; private set; }
        /// <summary>
        /// 与零时区的偏差时间
        /// </summary>
        public TimeSpan UtcOffset { get; private set; }
        /// <summary>
        /// 名称
        /// </summary>
        public LocaleValue StandardName { get; private set; }
        public string ForbidStatus { get; private set; }
        public string DocumentStatus { get; private set; }
        public bool CanBeUsed { get; private set; }
        #endregion
        #region Methods
        public KDTimeZone(long lId, string strNumber, LocaleValue localeValue, TimeSpan timeSpan, bool canBeUsed = true)
        {
            this.Id = lId;
            this.Number = strNumber;
            this.StandardName = localeValue;
            this.UtcOffset = timeSpan;
            this.CanBeUsed = canBeUsed;
        }

        public static Dictionary<long, KDTimeZone> ConvertToKDTimeZones(DynamicObjectCollection docTimeZones)
        {
            Dictionary<long, KDTimeZone> dicTimeZones;
            try
            {
                dicTimeZones = docTimeZones.Where(dy => null != dy["FTIMEOFFSET"]).Select(
                    dy =>
                    new KDTimeZone(
                        Convert.ToInt64(dy["FID"]),
                        dy["FNUMBER"].ToString(),
                        new LocaleValue(dy["FNAME"] == null ? "" : dy["FNAME"].ToString(), Convert.ToInt32(dy["FLOCALEID"])),
                        GetTimeSpan(0, Convert.ToInt32(dy["FTIMEOFFSET"]), 0),
                        (UnForbidStatus.Equals(dy["FFORBIDSTATUS"].ToString(), StringComparison.CurrentCultureIgnoreCase)
                        && AuditStatus.Equals(dy["FDOCUMENTSTATUS"].ToString(), StringComparison.CurrentCultureIgnoreCase))
                        )
                    ).ToDictionary(tz => tz.Id);
                if (dicTimeZones.Count == 0)
                {
                    throw new Exception("时区数据没有加载");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("时区数据加载不正确，请确认时区数据导入正确，信息如下：{0}", ex.Message));
            }

            return dicTimeZones;
        }
        public DateTime ConvertTime(DateTime dateTime, KDTimeZone destinationTimeZone)
        {
            if (this.Id == destinationTimeZone.Id)
            {
                return dateTime;
            }
            else
            {
                return dateTime - UtcOffset + destinationTimeZone.UtcOffset;
            }
        }
        private static TimeSpan GetTimeSpan(int iHours, int iMinutes, int iSeconds)
        {
            return new TimeSpan(iHours, iMinutes, iSeconds);
        }
        #endregion
    }
}
