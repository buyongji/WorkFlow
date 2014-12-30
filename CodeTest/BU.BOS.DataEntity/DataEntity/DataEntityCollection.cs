using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Orm.DataEntity
{
    /// <summary>
    /// 用于实体的集合，提供ListChanged事件及处理父子头条
    /// </summary>
    [Serializable]
    public class DataEntityCollection<T> : Collection<T>,ISupportInitializeNotification,IDeserializationCallback
    {
    }
}
