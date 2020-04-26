using System;
using System.Collections.Generic;
using System.Linq;

namespace BakedGoods.Dependency.Exjection.Utils
{
    public static class AssemblyScannerUtils
    {
        /// <summary>
        /// returns the portion of types that inherit from <T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="types"></param>
        /// <returns></returns>
        public static IEnumerable<Type> FilterImplementationsOfType<T>(IEnumerable<Type> types)
        {
            var findType = typeof(T);
            return FilterImplementationsOfType(findType, types);
        }

        public static IEnumerable<Type> FilterImplementationsOfType(Type ofType, IEnumerable<Type> types)
        {
            return types.Where(t => IsConcrete(t) && Inherits(ofType, t));
        }

        public static bool IsConcrete(Type t)
        {
            return !t.IsAbstract && !t.IsInterface;
        }

        public static bool Inherits<T>(Type t)
        {
            return Inherits(typeof(T), t);
        }

        public static bool Inherits(Type parent, Type t)
        {
            var res = parent.IsAssignableFrom(t);
            if (!res && t.IsGenericTypeDefinition && parent.IsGenericTypeDefinition)
            {
                foreach (var inheritType in new[] { t }.Union(GetInheritedTypes(t).Where(i => i.IsGenericType)))
                {
                    if (inheritType == parent ||
                        inheritType.Assembly == parent.Assembly && inheritType.GUID == parent.GUID)
                    {
                        return true;
                    }
                }
            }
            return res;
        }

        private static IEnumerable<Type> GetInheritedTypes(Type t)
        {

            foreach (var ifc in t.GetInterfaces())
            {
                yield return ifc;
            }
            if (t.BaseType != null && t.BaseType != typeof(object))
            {
                yield return t.BaseType;
                foreach (var tp in GetInheritedTypes(t.BaseType))
                {
                    yield return tp;
                }
            }
        }

        //https://stackoverflow.com/questions/457676/check-if-a-class-is-derived-from-a-generic-class
        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur || cur.GetInterfaces().Any(c => c == generic || generic.IsAssignableFrom(c)))
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        public static bool IsImplementationOfType<T>(Type t)
        {
            var findType = typeof(T);
            return Inherits(findType, t);
        }
    }
}
