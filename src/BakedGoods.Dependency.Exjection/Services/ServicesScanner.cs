using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BakedGoods.Dependency.Exjection.Abstractions.Services;
using BakedGoods.Dependency.Exjection.Utils;

namespace BakedGoods.Dependency.Exjection.Services
{
    public static class ServicesScanner
    {
        public static IEnumerable<ServicesScannerResult> GetImplementationsFiltered<T>(IEnumerable<Type> types, bool includeGivenType = false) where T : IInjectableService
        {
            var findType = typeof(T);
            foreach (var res in types.Select(c => GetImplementationsFiltered<T>(c, includeGivenType)).Where(c => c != null))
            {
                yield return res;
            }
        }

        public static ServicesScannerResult GetImplementationsFiltered<T>(Type checkingType, bool includeGivenType = false) where T : IInjectableService
        {
            var findType = typeof(T);
            if (AssemblyScannerUtils.IsImplementationOfType<T>(checkingType))
            {
                var interfaces = checkingType.GetInterfaces()
                    .Where(t => AssemblyScannerUtils.Inherits<T>(t))
                    .Where(t => includeGivenType || t != findType)
                    .Distinct().ToList();
                if (interfaces.Any())
                {
                    return new ServicesScannerResult(checkingType, interfaces);
                }
            }
            return null;
        }

        public static IEnumerable<ServicesScannerResult> GetImplementationsFiltered<T>(Assembly t, bool includeGivenType = false) where T : IInjectableService
        {
            return GetImplementationsFiltered<T>(t.ExportedTypes, includeGivenType);
        }
    }
}
