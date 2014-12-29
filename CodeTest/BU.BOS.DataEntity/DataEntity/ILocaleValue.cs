using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BU.BOS.Orm.DataEntity
{
    public interface ILocaleValue : IEnumerable<KeyValuePair<int, string>>, IEnumerable
    {
        string this[int lcid] { get; set; }
    }
}
