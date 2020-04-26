using System;

namespace BakedGoods.Dependency.Exjection.Abstractions.Configuration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ConfigurationSectionAttribute : Attribute
    {
        public ConfigurationSectionAttribute(string section)
        {
            if (string.IsNullOrWhiteSpace(section))
            {
                throw new Exception("Could not init ConfigurationSectionAttribute with empty string");
            }
            Section = section;
        }

        public string Section { get; }
    }
}
