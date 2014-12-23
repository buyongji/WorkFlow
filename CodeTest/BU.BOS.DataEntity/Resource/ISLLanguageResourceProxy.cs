using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BU.BOS.Resource
{
    public interface ISLLanguageResourceProxy
    {
        string LoadKDString(string description, string resourceId, string localId = "");
        bool TryGetKDString(string resourceId, out string value);

    }
}
