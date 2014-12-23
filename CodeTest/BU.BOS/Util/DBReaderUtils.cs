using BU.BOS.Resource;
using BU.BOS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Util
{
    public static class DBReaderUtils
    {
        // Methods
        private static void AddConvertFunc<T>(Dictionary<Type, object> funcs, Func<object, T> func)
        {
            funcs.Add(typeof(T), func);
        }

        public static T ConvertTo<T>(object value, Func<object, T> convertFunc)
        {
            return ConvertTo<T>(value, convertFunc, default(T));
        }

        public static T ConvertTo<T>(object value, Func<object, T> convertFunc, T defaultValue)
        {
            if (Convert.IsDBNull(value) || (value == null))
            {
                return defaultValue;
            }
            if (value is T)
            {
                return (T)value;
            }
            if (convertFunc == null)
            {
                convertFunc = ConvertFuncHelper<T>.Default;
            }
            if (convertFunc == null)
            {
                throw new ArgumentException(string.Format(ResManager.LoadKDString("数据{0}不能转换成{1}，请尝试提供转换器参数convertFunc或修正数据", "002016030001219", SubSystemType.BOS, new object[0]), value, typeof(T)));
            }
            return convertFunc(value);
        }

        private static Dictionary<Type, object> CreateDefaultConvertFunc()
        {
            Dictionary<Type, object> funcs = new Dictionary<Type, object>();
            AddConvertFunc<bool>(funcs, new Func<object, bool>(DBReaderUtils.ToBoolean));
            AddConvertFunc<byte>(funcs, new Func<object, byte>(Convert.ToByte));
            AddConvertFunc<char>(funcs, new Func<object, char>(Convert.ToChar));
            AddConvertFunc<DateTime>(funcs, new Func<object, DateTime>(Convert.ToDateTime));
            AddConvertFunc<decimal>(funcs, new Func<object, decimal>(Convert.ToDecimal));
            AddConvertFunc<double>(funcs, new Func<object, double>(Convert.ToDouble));
            AddConvertFunc<short>(funcs, new Func<object, short>(Convert.ToInt16));
            AddConvertFunc<int>(funcs, new Func<object, int>(Convert.ToInt32));
            AddConvertFunc<long>(funcs, new Func<object, long>(Convert.ToInt64));
            AddConvertFunc<sbyte>(funcs, new Func<object, sbyte>(Convert.ToSByte));
            AddConvertFunc<float>(funcs, new Func<object, float>(Convert.ToSingle));
            AddConvertFunc<string>(funcs, new Func<object, string>(Convert.ToString));
            AddConvertFunc<ushort>(funcs, new Func<object, ushort>(Convert.ToUInt16));
            AddConvertFunc<uint>(funcs, new Func<object, uint>(Convert.ToUInt32));
            AddConvertFunc<ulong>(funcs, new Func<object, ulong>(Convert.ToUInt64));
            AddConvertFunc<Guid>(funcs, new Func<object, Guid>(DBReaderUtils.ToGuid));
            return funcs;
        }

        public static bool GetBoolean(this IDataReader dr, string fieldName)
        {
            return Convert.ToBoolean(dr[fieldName]);
        }

        public static bool? GetBooleanEx(this IDataReader dr, string fieldName)
        {
            bool? nullable = null;
            if (dr[fieldName] != DBNull.Value)
            {
                nullable = new bool?(Convert.ToBoolean(dr[fieldName]));
            }
            return nullable;
        }

        public static DateTime GetDateTime(this IDataReader dr, string fieldName)
        {
            return Convert.ToDateTime(dr[fieldName]);
        }

        public static DateTime? GetDateTimeEx(this IDataReader dr, string fieldName)
        {
            DateTime? nullable = null;
            if (dr[fieldName] != DBNull.Value)
            {
                nullable = new DateTime?(Convert.ToDateTime(dr[fieldName]));
            }
            return nullable;
        }

        private static Func<object, T> GetDefaultConvertFunc<T>()
        {
            object obj2;
            if (CreateDefaultConvertFunc().TryGetValue(typeof(T), out obj2))
            {
                return (Func<object, T>)obj2;
            }
            return null;
        }

        public static int GetInt(this IDataReader dr, string fieldName)
        {
            return Convert.ToInt32(dr[fieldName]);
        }

        public static int GetIntCompatibleNull(this IDataReader dr, string fieldName, int NullConvert)
        {
            if (dr[fieldName] == DBNull.Value)
            {
                return NullConvert;
            }
            return Convert.ToInt32(dr[fieldName]);
        }

        public static int? GetIntEx(this IDataReader dr, string fieldName)
        {
            int? nullable = null;
            if (dr[fieldName] != DBNull.Value)
            {
                nullable = new int?(Convert.ToInt32(dr[fieldName]));
            }
            return nullable;
        }

        public static string GetString(this IDataReader dr, string fieldName)
        {
            return Convert.ToString(dr[fieldName]);
        }

        public static T GetValue<T>(this IDataRecord dr, int index)
        {
            return ConvertTo<T>(dr.GetValue(index), null);
        }

        public static T GetValue<T>(this IDataRecord dr, string fieldName)
        {
            return ConvertTo<T>(dr[fieldName], null);
        }

        public static T GetValue<T>(this IDataRecord dr, int index, Func<object, T> convertFunc)
        {
            return ConvertTo<T>(dr.GetValue(index), convertFunc);
        }

        public static T GetValue<T>(this IDataRecord dr, string fieldName, Func<object, T> convertFunc)
        {
            return ConvertTo<T>(dr[fieldName], convertFunc);
        }

        private static bool ToBoolean(object value)
        {
            if (value is string)
            {
                switch (((string)value))
                {
                    case string.Empty:
                    case "0":
                        return false;
                    case "1":
                    case "-1":
                        return true;
                }
            }
            return Convert.ToBoolean(value);
        }

        private static Guid ToGuid(object value)
        {
            byte[] b = value as byte[];
            if (b != null)
            {
                return new Guid(b);
            }
            string g = value as string;
            if (g == null)
            {
                throw new InvalidCastException(string.Format(ResManager.LoadKDString("无法将{0}转换为GUID类型", "002016030001222", SubSystemType.BOS, new object[0]), value));
            }
            return new Guid(g);
        }

        // Nested Types
        private static class ConvertFuncHelper<T>
        {
            // Fields
            public static readonly Func<object, T> Default;

            // Methods
            static ConvertFuncHelper()
            {
                DBReaderUtils.ConvertFuncHelper<T>.Default = DBReaderUtils.GetDefaultConvertFunc<T>();
            }
        }

    }
}
