using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryTest
{
    public class DisposeTest : IDisposable
    {
        private bool disposed = false;

        public object Clone()
        {
            return this.Clone();
        }

        /// <summary>
        /// 不是必要的，提供一个Close方法仅仅是为了更符合其他语言（如C++）的规范
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// 实现IDisposable中的Dispose方法
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 非密封类修饰用protected virtual
        /// 密封类修饰用private
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                return;
            }
            if (disposing)
            {
                //执行基本的清理代码
            }

            disposed = true;
        }

        /// <summary>
        /// 必须，以备程序员忘记了显式调用Dispose方法
        /// </summary>
        ~DisposeTest()
        {
            this.Dispose(false);
        }
    }
}
