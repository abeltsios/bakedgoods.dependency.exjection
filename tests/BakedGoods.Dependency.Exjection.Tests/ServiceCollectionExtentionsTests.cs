using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BakedGoods.Dependency.Exjection.Abstractions.Services;
using BakedGoods.Dependency.Exjection.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace BakedGoods.Common.DependencyInjection.Tests
{
    public class ServiceCollectionExtentionsTests
    {

        private interface IHost : IHostedServiceInterface
        {
            int Method();
        }

        private class HostedSer : BackgroundService, IHost
        {
            public static int InstanceCount { get; private set; }
            public HostedSer() : base()
            {
                InstanceCount++;
            }
            public int Method()
            {
                return InstanceCount;
            }

            protected override Task ExecuteAsync(CancellationToken stoppingToken)
            {
                return Task.CompletedTask;
            }
        }

        [Fact]
        public void ServiceCollectionExtentions_RegisterHostedServices_RegisterHostedServices_ShouldNotFail()
        {
            var collection = new ServiceCollection();
            collection.RegisterServicesByConvention(typeof(HostedSer));

            var serviceProvider = collection.BuildServiceProvider();

            var r = serviceProvider.GetRequiredService<IHost>().Method();
            Assert.Equal(1, r);

            r = serviceProvider.GetServices<IHostedService>().Count();
            Assert.Equal(1, r);

            r = (serviceProvider.GetServices<IHostedService>().Single() as IHost).Method();
            Assert.Equal(1, r);
        }

        public interface IUnusedInterface
        {
            int ReturnsNumber();
        }

        public interface IPlainTransient : IUnusedInterface, ITransientService
        {

        }

        public class PlainTransientImpl : IPlainTransient
        {
            public int ReturnsNumber()
            {
                return 4;
            }
        }

        public interface ITransientGenericInterface<T> : IUnusedInterface, ITransientService
        {

        }

        public class OpenTransientImplementation<T> : ITransientGenericInterface<T>
        {
            public int ReturnsNumber()
            {
                return 1;
            }
        }


        public interface IScopedGenericInterface<T> : IUnusedInterface, IScopedService
        {

        }

        public class OpenScopedGenericInterface<T> : IScopedGenericInterface<T>
        {
            public int ReturnsNumber()
            {
                return 2;
            }
        }

        public interface ISingletonGenericInterface<T> : IUnusedInterface, ISingletonService
        {

        }

        public class OpenSingletonGenericInterface<T> : ISingletonGenericInterface<T>
        {
            public int ReturnsNumber()
            {
                return 3;
            }
        }



        [Theory]
        [InlineData(typeof(ISingletonGenericInterface<string>), typeof(OpenSingletonGenericInterface<string>), 3, true)]
        [InlineData(typeof(IScopedGenericInterface<Guid>), typeof(OpenScopedGenericInterface<Guid>), 2)]
        [InlineData(typeof(ITransientGenericInterface<object>), typeof(OpenTransientImplementation<object>), 1)]
        [InlineData(typeof(IPlainTransient), typeof(PlainTransientImpl), 4)]
        public void ServiceCollectionExtentions_RegisterOpenGenericService_ShouldInstantiateAndGetZero(
            Type iface, Type impl,
            int expected, bool sameInstance = false)
        {
            var collection = new ServiceCollection();
            collection.RegisterServicesByConvention(
                typeof(OpenSingletonGenericInterface<>),
                typeof(OpenScopedGenericInterface<>),
                typeof(OpenTransientImplementation<>),
                typeof(PlainTransientImpl));

            var services = collection.BuildServiceProvider();
            var instanceOutter = services.GetRequiredService(iface) as IUnusedInterface;

            Assert.NotNull(instanceOutter);
            Assert.IsType(impl, instanceOutter);
            Assert.Equal(expected, instanceOutter.ReturnsNumber());

            using (var scope = services.CreateScope())
            {
                IUnusedInterface instance = scope.ServiceProvider.GetRequiredService(iface) as IUnusedInterface;
                Assert.NotNull(instance);
                Assert.IsType(impl, instance);
                Assert.Equal(expected, instance.ReturnsNumber());
                Assert.Equal(sameInstance, Object.ReferenceEquals(instance, instanceOutter));
            }
        }

    }
}
