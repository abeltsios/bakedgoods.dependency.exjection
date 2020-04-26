using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BakedGoods.Dependency.Exjection.Utils;
using Xunit;

namespace BakedGoods.Common.DependencyInjection.Tests
{
    public class AssemblyScannerUtilsTest
    {
        [Fact]
        public void AssemblyScannerUtils_SimpleInheritance_ShouldBeFound()
        {
            var result = AssemblyScannerUtils.FilterImplementationsOfType<IDisposable>(new[] { Task.CompletedTask.GetType() });
            Assert.Equal(Task.CompletedTask.GetType(), result.Single());
        }

        [Fact]
        public void AssemblyScannerUtils_NoInheritance_ShouldNotBeFound()
        {
            var result = AssemblyScannerUtils.FilterImplementationsOfType<IDisposable>(new[] { new object().GetType() });
            Assert.Empty(result);
        }

        [Fact]
        public void AssemblyScannerUtils_InterfaceAsInputAndType_ShouldNotBeFound()
        {
            var result = AssemblyScannerUtils.FilterImplementationsOfType<IDisposable>(new[] { typeof(IDisposable) });
            Assert.Empty(result);
        }

        [Fact]
        public void AssemblyScannerUtils_ClosedGenerics_ShouldBeFound()
        {
            var result = AssemblyScannerUtils.FilterImplementationsOfType<ICollection<string>>(new[] { typeof(List<string>) });
            Assert.NotEmpty(result);
        }

        [Fact]
        public void AssemblyScannerUtils_OpenGenerics_ShouldBeFound()
        {
            var ls = typeof(List<>).GetInterfaces().ToArray();

            var result = AssemblyScannerUtils.FilterImplementationsOfType(typeof(ICollection<>), new[] { typeof(List<>) }).ToList();
            Assert.NotEmpty(result);
        }


        public class RandomGenericClass<T>
        {

        }

        [Theory]
        [InlineData(typeof(IDisposable), typeof(Task), true)]
        [InlineData(typeof(IDisposable), typeof(object), false)]
        [InlineData(typeof(object), typeof(Task), true)]
        [InlineData(typeof(object), typeof(object), true)]
        [InlineData(typeof(Stream), typeof(MemoryStream), true)]
        [InlineData(typeof(IDisposable), typeof(MemoryStream), true)]
        [InlineData(typeof(IDisposable), typeof(Stream), false)]
        [InlineData(typeof(ICollection<>), typeof(List<>), true)]
        [InlineData(typeof(ICollection<>), typeof(List<string>), false)]
        [InlineData(typeof(ICollection<string>), typeof(List<string>), true)]
        [InlineData(typeof(RandomGenericClass<>), typeof(List<>), false)]
        [InlineData(typeof(RandomGenericClass<string>), typeof(List<string>), false)]
        public void AssemblyScannerUtils_TheοryFilterImplementationOfType_ShouldAgreeWithProvided(Type iface, Type implementation, bool found)
        {
            var result = AssemblyScannerUtils.FilterImplementationsOfType(iface, new[] { implementation }).ToList();
            if (found)
            {
                Assert.NotEmpty(result);
                Assert.Contains(result, c => c == implementation);
            }
            else
            {
                Assert.Empty(result);
            }
        }

    }
}
