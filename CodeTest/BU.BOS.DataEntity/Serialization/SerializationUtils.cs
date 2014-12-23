using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Serialization
{
    internal static class SerializationUtils
    {
        // Fields
        internal const string Schemas_Namespace = "http://schemas.kingdee.com/dataEntity/";

        // Methods
        internal static string IntArrayToString(int[] array)
        {
            if ((array == null) || (array.Length == 0))
            {
                return null;
            }
            StringBuilder builder = new StringBuilder(array.Length * 3);
            for (int i = 0; i < array.Length; i++)
            {
                if (i != 0)
                {
                    builder.Append(",");
                }
                builder.Append(array[i]);
            }
            return builder.ToString();
        }

        internal static int[] StringToIntArray(string arrayString)
        {
            if (string.IsNullOrEmpty(arrayString))
            {
                return null;
            }
            List<int> list = new List<int>(Math.Max(arrayString.Length / 3, 1));
            foreach (string str in arrayString.Split(new char[] { ',' }))
            {
                list.Add(int.Parse(str));
            }
            return list.ToArray();
        }

    }
}
