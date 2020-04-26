using System.Linq;
using BakedGoods.Dependency.Exjection.Abstractions.Services;
using BakedGoods.Dependency.Exjection.Services;
using Xunit;

namespace BakedGoods.Common.DependencyInjection.Tests
{
    public class ServicesScannerTests
    {
        private class TestT : IScopedService
        {

        }

        private interface IScopedServiceT : IScopedService
        {

        }


        private class TestT2 : IScopedServiceT
        {

        }



        private class TestT3 : IScopedServiceT
        {

        }

        private interface IServiceT : ITransientService
        {

        }

        private class TestT4 : IScopedServiceT, IServiceT
        {

        }

        private interface IGeneric<T> : ITransientService
        {

        }

        private class ImplementationGeneric<T> : IGeneric<T>, ITransientService
        {

        }

        [Fact]
        public void ServiceScanner_NoImplementation_NoneFound()
        {
            var res = ServicesScanner.GetImplementationsFiltered<IScopedService>(typeof(object));
            Assert.Null(res);
        }

        [Fact]
        public void ServiceScanner_NoImplementation_NotIncludeGivenType()
        {
            var res = ServicesScanner.GetImplementationsFiltered<IScopedService>(typeof(TestT), false);
            Assert.Null(res);
        }


        [Fact]
        public void ServiceScanner_NoImplementation_IncludeGivenType()
        {
            var res = ServicesScanner.GetImplementationsFiltered<IScopedService>(typeof(TestT), true);
            Assert.Equal(typeof(TestT), res.ImplementationType);
            Assert.Equal(typeof(IScopedService), res.ImplementingInterfaces.Single());
        }

        [Fact]
        public void ServiceScanner_ShouldGetIScopedServiceT()
        {
            var res = ServicesScanner.GetImplementationsFiltered<IScopedService>(typeof(TestT2));
            Assert.Equal(typeof(TestT2), res.ImplementationType);
            Assert.Equal(typeof(IScopedServiceT), res.ImplementingInterfaces.Single());
        }

        [Fact]
        public void ServiceScanner_Double_Interface_ShouldGetBoth()
        {
            var res = ServicesScanner.GetImplementationsFiltered<IScopedService>(typeof(TestT4));
            Assert.Equal(typeof(TestT4), res.ImplementationType);
            Assert.Equal(typeof(IScopedServiceT), res.ImplementingInterfaces.Single());

            res = ServicesScanner.GetImplementationsFiltered<ITransientService>(typeof(TestT4));
            Assert.Equal(typeof(TestT4), res.ImplementationType);
            Assert.Equal(typeof(IServiceT), res.ImplementingInterfaces.Single());
        }

        [Fact]
        public void ServiceScanner_GenericsOpenClosed_ShouldGetBoth()
        {
            var res = ServicesScanner.GetImplementationsFiltered<ITransientService>(typeof(ImplementationGeneric<>));
            Assert.Equal(typeof(ImplementationGeneric<>), res.ImplementationType);
            Assert.Equal(typeof(IGeneric<>).GUID, res.ImplementingInterfaces.Single().GUID);
            Assert.Equal(typeof(IGeneric<>).Assembly, res.ImplementingInterfaces.Single().Assembly);
        }


    }
}
