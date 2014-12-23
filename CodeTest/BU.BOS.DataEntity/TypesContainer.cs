using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS
{
  public  class TypesContainer
    {
        // Fields
        private static ConcurrentDictionary<Type, object> instancesDict = new ConcurrentDictionary<Type, object>();
        private static ConcurrentDictionary<string, Type> typesDict = new ConcurrentDictionary<string, Type>();

        // Methods
        public static Type GetOrRegister(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return null;
            }
            return typesDict.GetOrAdd(type, tp => Type.GetType(tp));
        }

        public static object GetOrRegisterSingletonInstance(string type)
        {
            Type orRegister = GetOrRegister(type);
            if (orRegister != null)
            {
                return instancesDict.GetOrAdd(orRegister, Activator.CreateInstance(orRegister));
            }
            return null;
        }

    }
}
