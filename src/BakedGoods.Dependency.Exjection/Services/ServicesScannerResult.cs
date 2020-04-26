using System;
using System.Collections.Generic;

namespace BakedGoods.Dependency.Exjection.Services
{
    public class ServicesScannerResult
    {
        public Type ImplementationType { get; }

        public IEnumerable<Type> ImplementingInterfaces => _interfacesInner;

        private readonly HashSet<Type> _interfacesInner = new HashSet<Type>();

        public ServicesScannerResult(Type t, IEnumerable<Type> ifaces)
        {
            ImplementationType = t;
            foreach (var c in ifaces)
            {
                if (!_interfacesInner.Add(c))
                {
                    throw new Exception($"type {c.FullName} was already added as implementing interface of {t.FullName}");
                }
            }
        }
    }
}
