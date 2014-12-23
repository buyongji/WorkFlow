using BU.BOS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.JSON
{
    [Serializable]
    public class JSONObject : Dictionary<string, object>
    {
        #region Methods
        public JSONObject()
        {

        }
        protected JSONObject(SerializationInfo info,StreamingContext context):base(info,context)
        {

        }
        public object Get(string strPropName)
        {
            object obj = null;
            base.TryGetValue(strPropName,out obj);
            return obj;
        }
        public bool GetBool(string strPropName)
        {
            object obj = false;
            base.TryGetValue(strPropName,out obj);
            return Convert.ToBoolean(obj);
        }
        public int GetInt(string strPropName)
        {
            object obj = null;
            if(base.TryGetValue(strPropName,out obj))
            {
                return Convert.ToInt32(obj);
            }
            return 0;
        }
        public JSONObject GetJSONObject(string strPropName)
        {
            object obj = null;
            base.TryGetValue(strPropName,out obj);
            return (obj as JSONObject);
        }
        public long GetLong(string propName)
        {
            return (long)base[propName];
        }
        public string GetString(string propName)
        {
            object obj2 = null;
            base.TryGetValue(propName, out obj2);
            return (string)obj2;
        }
        public T GetValue<T>(string key, T defaultValue)
        {
            object obj2;
            if (!base.TryGetValue(key, out obj2))
            {
                return defaultValue;
            }
            if (((obj2 != null) && (obj2.GetType().Name == typeof(long).Name)) && (typeof(T) == typeof(int)))
            {
                obj2 = int.Parse(obj2.ToString());
            }
            return DBReaderUtils.ConvertTo<T>(obj2, null);
        }
        public void Put(string propName, object value)
        {
            base[propName] = value;
        }
        public string ToJSONString()
        {
            return null;
        }
        public bool TryGetValue(string key, out object v, bool removeItem)
        {
            if (!base.TryGetValue(key, out v))
            {
                return false;
            }
            if (removeItem)
            {
                base.Remove(key);
            }
            return true;
        }

        public bool TryGetValue<T>(string key, T defaultValue, out T retValue)
        {
            object obj2;
            bool flag = base.TryGetValue(key, out obj2);
            if (flag)
            {
                if (((obj2 != null) && (obj2.GetType().Name == typeof(long).Name)) && (typeof(T) == typeof(int)))
                {
                    obj2 = int.Parse(obj2.ToString());
                }
                retValue = DBReaderUtils.ConvertTo<T>(obj2, null);
                return flag;
            }
            retValue = defaultValue;
            return flag;
        }
        #endregion
    }
}
