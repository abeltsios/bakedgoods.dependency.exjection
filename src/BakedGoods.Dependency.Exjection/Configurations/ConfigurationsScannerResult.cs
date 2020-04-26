using System;

namespace BakedGoods.Dependency.Exjection.Configurations
{
    // https://stackoverflow.com/questions/31453495/how-to-read-appsettings-values-from-json-file-in-asp-net-core
    // for registration
    public class ConfigurationsScannerResult
    {
        public Type ConfigurationType { get; }

        public string SectionName { get; }

        public ConfigurationsScannerResult(Type configurationType, string sectionName)
        {
            ConfigurationType = configurationType ?? throw new ArgumentNullException(nameof(configurationType));
            SectionName = sectionName ?? throw new ArgumentNullException(nameof(sectionName));
        }
    }
}
