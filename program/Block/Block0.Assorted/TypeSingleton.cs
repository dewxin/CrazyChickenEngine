using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Block.Assorted
{
    public static class TypeSingleton
    {
        public static Dictionary<Type, object> singletonDict = new Dictionary<Type, object>();

        public static void Set<T>(object single)
        {
            singletonDict.Add(typeof(T), single);
        }

        public static T Get<T>()
            where T:class
        {
            if (singletonDict.TryGetValue(typeof(T), out var t))
                return (T)t;

            return null;
        }


    }
}
