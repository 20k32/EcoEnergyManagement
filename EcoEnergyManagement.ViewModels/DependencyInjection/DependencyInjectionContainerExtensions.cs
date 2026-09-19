using EcoEnergyManagement.Models.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EcoEnergyManagement.ViewModels.DependencyInjection
{
    public static class DependencyInjectionContainerExtensions
    {
        public static IServiceCollection RegisterViewModelServices(this IServiceCollection services)
            => services.RegisterModelsServices();
    }
}
