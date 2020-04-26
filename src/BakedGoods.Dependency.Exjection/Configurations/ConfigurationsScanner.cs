using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BakedGoods.Dependency.Exjection.Abstractions.Configuration;
using BakedGoods.Dependency.Exjection.Utils;

namespace BakedGoods.Dependency.Exjection.Configurations
{

    public static class ConfigurationsScanner
    {
        public static IEnumerable<ConfigurationsScannerResult> GetConfigurationFiltered(IEnumerable<Type> types)
        {
            return types
                        .Select(GetConfigurationFiltered)
                        .Where(c => c != null)
                        .ToList();
        }

        public static ConfigurationsScannerResult GetConfigurationFiltered(Type configType)
        {
            if (AssemblyScannerUtils.Inherits<IConfigurationBase>(configType))
            {
                var section = Attribute.GetCustomAttribute(configType, typeof(ConfigurationSectionAttribute)) as ConfigurationSectionAttribute;
                if (section != null)
                {
                    return new ConfigurationsScannerResult(configType, section.Section);
                }
            }

            return null;
        }

        public static IEnumerable<ConfigurationsScannerResult> GetConfigurationFiltered(Assembly t)
        {
            return GetConfigurationFiltered(t.ExportedTypes);
        }
    }
}
