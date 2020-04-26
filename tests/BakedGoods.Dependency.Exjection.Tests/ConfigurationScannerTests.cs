using BakedGoods.Dependency.Exjection.Abstractions.Configuration;
using BakedGoods.Dependency.Exjection.Configurations;
using Xunit;

namespace BakedGoods.Common.DependencyInjection.Tests
{
    public class ConfigurationScannerTests
    {
        [ConfigurationSection("lala")]
        private class AConfigClass : IConfigurationBase
        {

        }

        [Fact]
        public void ConfigurationScannerTests_ShouldGetNone()
        {
            var res = ConfigurationsScanner.GetConfigurationFiltered(typeof(object));
            Assert.Null(res);
        }

        [Fact]
        public void ConfigurationScannerTests_ShouldGetExpected()
        {
            var res = ConfigurationsScanner.GetConfigurationFiltered(typeof(AConfigClass));
            Assert.NotNull(res);
            Assert.Equal("lala", res.SectionName);
            Assert.Equal(typeof(AConfigClass), res.ConfigurationType);
        }
    }
}
