using Microsoft.Extensions.Hosting;

namespace BakedGoods.Dependency.Exjection.Abstractions.Services
{
    /// <summary>
    /// interface for "per call" service
    /// </summary>
    public interface IHostedServiceInterface : IHostedService, IInjectableService
    {
    }
}
