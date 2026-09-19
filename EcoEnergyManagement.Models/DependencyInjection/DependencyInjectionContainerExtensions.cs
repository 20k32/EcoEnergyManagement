using EcoEnergyManagement.Core.DependencyInjection;
using EcoEnergyManagement.PlatformHelpers.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EcoEnergyManagement.Models.DependencyInjection
{
    public static class DependencyInjectionContainerExtensions
    {
        public static IServiceCollection RegisterModelsServices(this IServiceCollection services)
            => services.RegisterCoreServices()
            .RegisterPlatformHelpers();
    }
}
