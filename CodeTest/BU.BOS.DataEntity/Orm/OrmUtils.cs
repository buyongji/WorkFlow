using BU.BOS.Orm.DataEntity;
using BU.BOS.Orm.Metadata.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        DcxmlSerializer serializer = new DcxmlSerializer(new IDataEntityType[] { dt }) {
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
            return (T) primaryKey.GetValue(dataEntity);
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

   

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<Kingdee.BOS.Orm.Metadata.DataEntity.IComplexProperty>.GetEnumerator();
        }

        [DebuggerHidden]
        void IEnumerator.Reset()
        {
            throw new NotSupportedException();
        }

        void IDisposable.Dispose()
        {
        }

        // Properties
        IComplexProperty IEnumerator<IComplexProperty>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.<>2__current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.<>2__current;
            }
        }
    }

    [CompilerGenerated]
    private sealed class <GetSimpleProperties>d__0 : IEnumerable<ISimpleProperty>, IEnumerable, IEnumerator<ISimpleProperty>, IEnumerator, IDisposable
    {
        // Fields
        private int <>1__state;
        private ISimpleProperty <>2__current;
        public bool <>3__onlyDbProperty;
        public IDataEntityPropertyCollection <>3__properties;
        private int <>l__initialThreadId;
        public int <i>5__3;
        public IDataEntityProperty <p>5__2;
        public ISimpleProperty <sp>5__1;
        public bool onlyDbProperty;
        public IDataEntityPropertyCollection properties;

        // Methods
        [DebuggerHidden]
        public <GetSimpleProperties>d__0(int <>1__state)
        {
            this.<>1__state = <>1__state;
            this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        private bool MoveNext()
        {
            switch (this.<>1__state)
            {
                case 0:
                    this.<>1__state = -1;
                    this.<i>5__3 = 0;
                    goto Label_0099;

                case 1:
                    this.<>1__state = -1;
                    break;

                default:
                    goto Label_00AF;
            }
        Label_008B:
            this.<i>5__3++;
        Label_0099:
            if (this.<i>5__3 < this.properties.Count)
            {
                this.<p>5__2 = this.properties[this.<i>5__3];
                this.<sp>5__1 = this.<p>5__2 as ISimpleProperty;
                if ((this.<sp>5__1 != null) && (!this.onlyDbProperty || !this.<p>5__2.IsDbIgnore()))
                {
                    this.<>2__current = this.<sp>5__1;
                    this.<>1__state = 1;
                    return true;
                }
                goto Label_008B;
            }
        Label_00AF:
            return false;
        }

        [DebuggerHidden]
        IEnumerator<ISimpleProperty> IEnumerable<ISimpleProperty>.GetEnumerator()
        {
            OrmUtils.<GetSimpleProperties>d__0 d__;
            if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
            {
                this.<>1__state = 0;
                d__ = this;
            }
            else
            {
                d__ = new OrmUtils.<GetSimpleProperties>d__0(0);
            }
            d__.properties = this.<>3__properties;
            d__.onlyDbProperty = this.<>3__onlyDbProperty;
            return d__;
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<Kingdee.BOS.Orm.Metadata.DataEntity.ISimpleProperty>.GetEnumerator();
        }

        [DebuggerHidden]
        void IEnumerator.Reset()
        {
            throw new NotSupportedException();
        }

        void IDisposable.Dispose()
        {
        }

        // Properties
        ISimpleProperty IEnumerator<ISimpleProperty>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.<>2__current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.<>2__current;
            }
        }
    }
}

}
