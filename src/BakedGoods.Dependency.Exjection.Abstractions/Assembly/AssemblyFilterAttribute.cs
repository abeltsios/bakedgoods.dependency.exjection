using System;

namespace BakedGoods.Dependency.Exjection.Abstractions.Assembly
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public sealed class AssemblyFilterAttribute : Attribute
    {
        public bool IncludeCurrentAssembly { get; }
        public AssemblyFilterAttribute(bool includeCurrentAssembly = true)
        {

        }
    }
}
