
# bakedgoods.dependency.exjection
A pet project of a pet project.

More or less, a bunch of interfaces and attributes
that help you to register services on Microsoft.Extensions.DependencyInjection.IServiceCollection

## IHostedServiceInterface
registers a hosted service. The implemetation itself is registered as a singleton

## IScopedService
Registers service as a scoped dependency


## ISingletonService
Registers service as a singleton

## ITransientService
Registers service as a transient dependency. (aka instance pes dependency)



### Examples

Given that you have defined an interface and an implementation
```
        //an IGeneric<T> interface that
        //is flagged as an ITransientService
        //(aka) instance per depencency
        public interface IGeneric<T> : ITransientService
        {

        }

        //The implementation of IGeneric<T>
        public class ImplementationGeneric<T> : IGeneric<T>, ITransientService
        {

        }

        public interface IScopedInterface : IUnusedInterface, IScopedService
        {
            int ReturnsNumber()
        }

        public class ScopedImpl : IScopedInterface 
        {
            public int ReturnsNumber()
            {
                return 2;
            }
        }
```

You can then register it on your service Collection by
```
var collection = new ServiceCollection();
collection.RegisterServicesByConvention(
   typeof(ImplementationGeneric<>),
   typeof(ScopedImpl));
```

The RegisterServicesByConvention extension method has overloads that works with Assemblies (but only checks exported classes)
