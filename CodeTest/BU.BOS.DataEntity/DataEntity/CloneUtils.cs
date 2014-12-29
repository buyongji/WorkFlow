using BU.BOS.Orm.Exceptions;
using BU.BOS.Orm.Metadata.DataEntity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Orm.DataEntity
{
    internal sealed class CloneUtils
    {
        // Fields
        private bool _clearPrimaryKeyValue;
        private bool _onlyDbProperty;

        // Methods
        public CloneUtils(bool onlyDbProperty, bool clearPrimaryKeyValue)
        {
            this._onlyDbProperty = onlyDbProperty;
            this._clearPrimaryKeyValue = clearPrimaryKeyValue;
        }

        public object Clone(IDataEntityBase dataEntity)
        {
            IDataEntityType dataEntityType = dataEntity.GetDataEntityType();
            return this.Clone(dataEntityType, dataEntity);
        }

        public object Clone(IDataEntityType dt, object dataEntity)
        {
            return this.Clone(dt, dataEntity, this._clearPrimaryKeyValue);
        }

        private object Clone(IDataEntityType dt, object dataEntity, bool clearPrimaryKeyValue)
        {
            if (dataEntity == null)
            {
                return null;
            }
            object newEntity = dt.CreateInstance();
            this.CopyData(dt, dataEntity, newEntity, clearPrimaryKeyValue);
            return newEntity;
        }

        private void CopyData(IDataEntityType dt, object dataEntity, object newEntity, bool clearPrimaryKeyValue)
        {
            IDataEntityType objA = dt;
            IDataEntityType objB = dt;
            if (dataEntity is IDataEntityBase)
            {
                objA = (dataEntity as IDataEntityBase).GetDataEntityType();
            }
            if (newEntity is IDataEntityBase)
            {
                objB = (newEntity as IDataEntityBase).GetDataEntityType();
            }
            if ((dataEntity is DynamicObject) && object.ReferenceEquals(objA, objB))
            {
                ((DynamicObject)newEntity).DataStorage = ((DynamicObject)dataEntity).DataStorage.MemberClone();
            }
            else
            {
                foreach (ISimpleProperty property in dt.Properties.GetSimpleProperties(this._onlyDbProperty))
                {
                    IDataEntityProperty dpOldProperty = null;
                    this.TryGetOldProperty(property, objA, out dpOldProperty);
                    if (!property.IsReadOnly && (dpOldProperty != null))
                    {
                        property.SetValue(newEntity, dpOldProperty.GetValue(dataEntity));
                    }
                }
            }
            if (clearPrimaryKeyValue)
            {
                ISimpleProperty primaryKey = dt.PrimaryKey;
                if (primaryKey != null)
                {
                    primaryKey.ResetValue(newEntity);
                }
                objB.SetDirty(newEntity, true);
            }
            foreach (IComplexProperty property4 in dt.Properties.GetComplexProperties(this._onlyDbProperty))
            {
                if (!property4.IsRefrenceObject())
                {
                    IDataEntityProperty property5 = null;
                    this.TryGetOldProperty(property4, objA, out property5);
                    object obj2 = property5.GetValue(dataEntity);
                    if (obj2 != null)
                    {
                        object obj3;
                        IDataEntityType dataEntityType;
                        IDataEntityBase base2 = obj2 as IDataEntityBase;
                        if (base2 != null)
                        {
                            dataEntityType = base2.GetDataEntityType();
                        }
                        else
                        {
                            dataEntityType = property4.ComplexPropertyType;
                        }
                        if (property4.IsReadOnly)
                        {
                            obj3 = property4.GetValue(newEntity);
                            if (obj3 == null)
                            {
                                throw new ORMDesignException("??????", ResManager.LoadKDString("哦，真不幸，只读的属性却返回了NULL值。", "014009000001633", SubSystemType.SL, new object[0]));
                            }
                            this.CopyData(dataEntityType, obj2, obj3, false);
                        }
                        else
                        {
                            if (property4.IsDbIgnore())
                            {
                                obj3 = this.Clone(dataEntityType, obj2, false);
                            }
                            else
                            {
                                obj3 = this.Clone(dataEntityType, obj2, this._clearPrimaryKeyValue);
                            }
                            property4.SetValue(newEntity, obj3);
                        }
                    }
                }
            }
            foreach (ICollectionProperty property6 in dt.Properties.GetCollectionProperties(this._onlyDbProperty))
            {
                IDataEntityProperty property7 = null;
                this.TryGetOldProperty(property6, objA, out property7);
                object obj4 = property7.GetValue(dataEntity);
                if (obj4 != null)
                {
                    IEnumerable enumerable = obj4 as IEnumerable;
                    if (enumerable == null)
                    {
                        throw new ORMDesignException("??????", ResManager.LoadKDString("哦，真不幸，集合的属性返回值不支持枚举。", "014009000001634", SubSystemType.SL, new object[0]));
                    }
                    if (newEntity is DynamicObject)
                    {
                        ((DynamicObject)newEntity).DataStorage.SetLocalValue((DynamicProperty)property6, null);
                    }
                    object obj5 = property6.GetValue(newEntity);
                    if (obj5 == null)
                    {
                        if (property6.IsReadOnly)
                        {
                            throw new ORMDesignException("??????", ResManager.LoadKDString("哦，真不幸，集合的属性返回值为null。", "014009000001635", SubSystemType.SL, new object[0]));
                        }
                        obj5 = Activator.CreateInstance(property6.PropertyType);
                        property6.SetValue(newEntity, obj5);
                    }
                    IList list = obj5 as IList;
                    if (list == null)
                    {
                        throw new ORMDesignException("??????", ResManager.LoadKDString("哦，真不幸，集合的属性返回值不支持IList。", "014009000001636", SubSystemType.SL, new object[0]));
                    }
                    list.Clear();
                    foreach (object obj6 in enumerable)
                    {
                        IDataEntityType collectionItemPropertyType;
                        IDataEntityBase base3 = obj6 as IDataEntityBase;
                        if (base3 == null)
                        {
                            collectionItemPropertyType = property6.CollectionItemPropertyType;
                        }
                        else
                        {
                            collectionItemPropertyType = base3.GetDataEntityType();
                        }
                        list.Add(this.Clone(collectionItemPropertyType, obj6));
                    }
                }
            }
        }

        private bool TryGetOldProperty(IDataEntityProperty dp, IDataEntityType dtOldData, out IDataEntityProperty dpOldProperty)
        {
            dpOldProperty = null;
            return (((dtOldData != null) && (dp != null)) && dtOldData.Properties.TryGetValue(dp.Name, out dpOldProperty));
        }
    }




}
