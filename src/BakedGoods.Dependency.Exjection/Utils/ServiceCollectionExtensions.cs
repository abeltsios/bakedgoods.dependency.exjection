using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BakedGoods.Dependency.Exjection.Abstractions.Assembly;
using BakedGoods.Dependency.Exjection.Abstractions.Configuration;
using BakedGoods.Dependency.Exjection.Abstractions.Services;
using BakedGoods.Dependency.Exjection.Configurations;
using BakedGoods.Dependency.Exjection.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BakedGoods.Dependency.Exjection.Utils
{
    public static class ServiceCollectionExtensions
    {
        public delegate void RegistrationCallback(IServiceCollection collection, Type interfaceType, Type Implementation);

        private static MethodInfo _registerHostedServiceMethod;

        private static MethodInfo GetGenericMethodForHostedServiceRegistration(Type interfacetype, Type implementation)
        {
            return _registerHostedServiceMethod.MakeGenericMethod(new[] { interfacetype, implementation });
        }

        private static IServiceCollection AddToServiceCollectionWithCallback(
            this IServiceCollection serviceCollection,
            IEnumerable<ServicesScannerResult> services,
            RegistrationCallback registration,
            RegistrationCallback multipleInterfacesRegistration = null)
        {
            foreach (var s in services)
            {
                var ifaces = s.ImplementingInterfaces.ToList();
                var multipleMode = ifaces.Count > 1 && multipleInterfacesRegistration != null;

                foreach (var iFace in ifaces)
                {
                    var ifFaceToAdd = iFace;
                    var impl = s.ImplementationType;

                    if (iFace.IsGenericType && iFace.GetGenericArguments().All(c => c.IsGenericParameter))
                    {
                        ifFaceToAdd = ifFaceToAdd.GetGenericTypeDefinition();
                    }

                    if (s.ImplementationType.IsGenericType && s.ImplementationType.GetGenericArguments().All(c => c.IsGenericParameter))
                    {
                        impl = impl.GetGenericTypeDefinition();
                    }

                    if (!multipleMode)
                    {
                        registration(serviceCollection, ifFaceToAdd, impl);
                    }
                    else
                    {
                        multipleInterfacesRegistration(serviceCollection, ifFaceToAdd, impl);
                    }
                }
            }
            return serviceCollection;
        }

        static ServiceCollectionExtensions()
        {
            _registerHostedServiceMethod = typeof(ServiceCollectionExtensions).GetMethod(nameof(ServiceCollectionExtensions.RegisterHostedService))
                ?? throw new MissingMethodException(nameof(ServiceCollectionExtensions.RegisterHostedService));
        }

        public static IServiceCollection RegisterTransientServices(this IServiceCollection serviceCollection, IEnumerable<ServicesScannerResult> services)
        {
            return serviceCollection.AddToServiceCollectionWithCallback(services, (col, iface, impl) => { col.AddTransient(iface, impl); });
        }

        public static IServiceCollection RegisterScopedServices(this IServiceCollection serviceCollection, IEnumerable<ServicesScannerResult> services)
        {
            return serviceCollection.AddToServiceCollectionWithCallback(services, (col, iface, impl) => { col.AddScoped(iface, impl); });
        }

        public static IServiceCollection RegisterSingletonServices(this IServiceCollection serviceCollection, IEnumerable<ServicesScannerResult> services)
        {
            return serviceCollection.AddToServiceCollectionWithCallback(services, (col, iface, impl) => { col.AddSingleton(iface, impl); });
        }

        public static IServiceCollection RegisterHostedServices(this IServiceCollection serviceCollection, IEnumerable<ServicesScannerResult> services)
        {
            foreach (var s in services)
            {
                foreach (var iFace in s.ImplementingInterfaces)
                {
                    GetGenericMethodForHostedServiceRegistration(iFace, s.ImplementationType).Invoke(null, new object[] {
                        serviceCollection});
                }
            }

            return serviceCollection;
        }

        public static IServiceCollection RegisterHostedService<TService, TImplementation>(this IServiceCollection services)
            where TService : class, IHostedService
            where TImplementation : class, TService
        {
            return services
                 .AddSingleton<TService, TImplementation>()
                 .AddHostedService<TService>(c => c.GetRequiredService<TService>());

        }

        public static IServiceCollection RegisterServicesByConvention(this IServiceCollection services, bool useAssemblyFilterAttribute = false, params Assembly[] assemblies)
        {
            var types = assemblies.Where(c => !useAssemblyFilterAttribute 
            || (Attribute.GetCustomAttribute(c, typeof(AssemblyFilterAttribute)) as AssemblyFilterAttribute)?.IncludeCurrentAssembly == true)
                .SelectMany(c => c.GetExportedTypes()).ToArray();
            return services.RegisterServicesByConvention(types);
        }

        // todo: decide how to handle di overrides
        public static IServiceCollection RegisterServicesByConvention(this IServiceCollection services, params Type[] types)
        {
            services
                .RegisterSingletonServices(ServicesScanner.GetImplementationsFiltered<ISingletonService>(types))
                .RegisterHostedServices(ServicesScanner.GetImplementationsFiltered<IHostedServiceInterface>(types))
                .RegisterScopedServices(ServicesScanner.GetImplementationsFiltered<IScopedService>(types))
                .RegisterTransientServices(ServicesScanner.GetImplementationsFiltered<ITransientService>(types));

            return services;
        }

        public static IServiceCollection RegisterConfiguration<T>(this IServiceCollection services, IConfiguration config, Action<T> configure = null)
             where T : class, IConfigurationBase, new()
        {
            var res = ConfigurationsScanner.GetConfigurationFiltered(typeof(T));
            if (res == null)
            {
                throw new ArgumentException($"{typeof(T).Name} does not comply with conventions!");
            }
            var optsBuilder = services.AddOptions<T>();

            if (configure != null)
            {
                optsBuilder = optsBuilder.Configure(configure);
            }

            optsBuilder.Bind(config.GetSection(res.SectionName));

            return services;
        }

    }
}
