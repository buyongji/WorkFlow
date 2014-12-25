using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Orm.DataEntity
{
    /// <summary>
    /// 实体枚举器的回调
    /// </summary>
    /// <param name="e"></param>
    public delegate void DataEntityWalkerCallback(DataEntityWalkerEventArgs e);
    /// <summary>
    /// 实体枚举器的回调参数对象
    /// </summary>
    public sealed class DataEntityWalkerEventArgs : EventArgs
    {
        #region Properties
        /// <summary>
        /// 返回当前回调事件中实体的部数目
        /// </summary>
        [ReadOnly(true)]
        public int Count
        {
            get;
            private set;
        }
        #endregion
        #region Methods
        #endregion
    }
}
