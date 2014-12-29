using BU.BOS.Orm.DataEntity;
using BU.BOS.Orm.Metadata.DataEntity;
using BU.BOS.Resource;
using BU.BOS.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BU.BOS.Orm
{
    public static class OrmUtils
    {
        // Methods
        public static object Clone(this IDataEntityBase dataEntity, bool onlyDbProperty = false, bool clearPrimaryKeyValue = true)
        {
            return new CloneUtils(onlyDbProperty, clearPrimaryKeyValue).Clone(dataEntity);
        }

        public static object Clone(this object dataEntity, IDataEntityType dt, bool onlyDbProperty = false, bool clearPrimaryKeyValue = false)
        {
            if (dataEntity == null)
            {
                return null;
            }
            if (dt == null)
            {
                throw new ArgumentNullException("dt");
            }
            return new CloneUtils(onlyDbProperty, clearPrimaryKeyValue).Clone(dt, dataEntity);
        }

        public static StringBuilder DataEntityToString(IDataEntityType dt, object dataEntity)
        {
            StringBuilder sb = new StringBuilder();
            DcxmlSerializer serializer = new DcxmlSerializer(new IDataEntityType[] { dt })
            {
                Binder = { OnlyDbProperty = false }
            };
            using (StringWriter writer = new StringWriter(sb))
            {
                using (XmlTextWriter writer2 = new XmlTextWriter(writer))
                {
                    serializer.Serialize(writer2, dataEntity, null);
                }
            }
            return sb;
        }

        public static void DataEntityWalker(IEnumerable<object> dataEntities, IDataEntityType dt, DataEntityWalkerCallback callback, bool onlyDbProperty = true)
        {
            DataEntityWalkerEventArgs.DataEntityWalker(dataEntities, dt, callback, onlyDbProperty);
        }

        public static IEnumerable<ICollectionProperty> GetCollectionProperties(this IDataEntityPropertyCollection properties, bool onlyDbProperty = true)
        {
            for (int i = 0; i < properties.Count; i++)
            {
                IDataEntityProperty metadata = properties[i];
                ICollectionProperty iteratorVariable0 = metadata as ICollectionProperty;
                if ((iteratorVariable0 != null) && (!onlyDbProperty || !metadata.IsDbIgnore()))
                {
                    yield return iteratorVariable0;
                }
            }
        }

        public static IEnumerable<IComplexProperty> GetComplexProperties(this IDataEntityPropertyCollection properties, bool onlyDbProperty = true)
        {
            for (int i = 0; i < properties.Count; i++)
            {
                IDataEntityProperty metadata = properties[i];
                IComplexProperty iteratorVariable0 = metadata as IComplexProperty;
                if ((iteratorVariable0 != null) && (!onlyDbProperty || !metadata.IsDbIgnore()))
                {
                    yield return iteratorVariable0;
                }
            }
        }

        public static IDataEntityType GetDataEntityType(this Type type)
        {
            return DataEntityType.GetDataEntityType(type);
        }

        internal static bool GetListEquals<T>(IEnumerable<T> x, IEnumerable<T> y)
        {
            if ((x == null) || (y == null))
            {
                return ((x == null) && (y == null));
            }
            IEnumerator<T> enumerator = x.GetEnumerator();
            IEnumerator<T> enumerator2 = y.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!enumerator2.MoveNext())
                {
                    return false;
                }
                if (!object.Equals(enumerator.Current, enumerator2.Current))
                {
                    return false;
                }
            }
            return true;
        }

        internal static int GetListHashCode<T>(IEnumerable<T> list)
        {
            int num = 0;
            if (list != null)
            {
                foreach (T local in list)
                {
                    num ^= local.GetHashCode();
                }
            }
            return num;
        }

        public static object GetPrimaryKeyValue(this IDataEntityBase dataEntity, bool throwOnError = true)
        {
            if (dataEntity == null)
            {
                if (throwOnError)
                {
                    throw new ArgumentNullException("dataEntity");
                }
                return null;
            }
            ISimpleProperty primaryKey = dataEntity.GetDataEntityType().PrimaryKey;
            if (primaryKey != null)
            {
                return primaryKey.GetValue(dataEntity);
            }
            if (throwOnError)
            {
                throw new NotSupportedException(string.Format(ResManager.LoadKDString("实体类型{0}沒有定义主键，无法获取。", "014009000001732", SubSystemType.SL, new object[0]), dataEntity.GetDataEntityType().Name));
            }
            return null;
        }

        public static T GetPrimaryKeyValue<T>(this IDataEntityBase dataEntity, bool throwOnError = true)
        {
            if (dataEntity == null)
            {
                if (throwOnError)
                {
                    throw new ArgumentNullException("dataEntity");
                }
                return default(T);
            }
            ISimpleProperty primaryKey = dataEntity.GetDataEntityType().PrimaryKey;
            if (primaryKey != null)
            {
                return (T)primaryKey.GetValue(dataEntity);
            }
            if (throwOnError)
            {
                throw new NotSupportedException(string.Format(ResManager.LoadKDString("实体类型{0}沒有定义主键，无法获取。", "014009000001732", SubSystemType.SL, new object[0]), dataEntity.GetDataEntityType().Name));
            }
            return default(T);
        }

        public static IEnumerable<ISimpleProperty> GetSimpleProperties(this IDataEntityPropertyCollection properties, bool onlyDbProperty = true)
        {
            for (int i = 0; i < properties.Count; i++)
            {
                IDataEntityProperty metadata = properties[i];
                ISimpleProperty iteratorVariable0 = metadata as ISimpleProperty;
                if ((iteratorVariable0 != null) && (!onlyDbProperty || !metadata.IsDbIgnore()))
                {
                    yield return iteratorVariable0;
                }
            }
        }

        public static bool IsDbIgnore(this IMetadata metadata)
        {
            IIsDefinedDbIgnoreAttribute attribute = metadata as IIsDefinedDbIgnoreAttribute;
            if (attribute != null)
            {
                return attribute.IsDefinedDbIgnoreAttribute();
            }
            return metadata.IsDefined(typeof(DbIgnoreAttribute), true);
        }

        public static bool IsRefrenceObject(this IMetadata metadata)
        {
            return metadata.IsDefined(typeof(RefrenceObjectAttribute), true);
        }

        public static object StringToDataEntity(IDataEntityType dt, string xml)
        {
            DcxmlSerializer serializer = new DcxmlSerializer(new IDataEntityType[] { dt });
            return serializer.DeserializeFromString(xml, null);
        }

        public static void Sync<SourceT, TargetT>(IEnumerable<SourceT> sourceList, IEnumerable<TargetT> targetList, Func<SourceT, TargetT, bool> equatable, Action<SourceT, TargetT> updateFunc, Func<SourceT, TargetT> createFunc, Action<IEnumerable<TargetT>, TargetT> addFunc = null, Action<IEnumerable<TargetT>, TargetT, int> removeFunc = null, bool callUpdateFuncWhenCreated = true)
        {
            ListSync.Sync<SourceT, TargetT>(sourceList, targetList, equatable, updateFunc, createFunc, addFunc, removeFunc, callUpdateFuncWhenCreated);
        }

    }
}
