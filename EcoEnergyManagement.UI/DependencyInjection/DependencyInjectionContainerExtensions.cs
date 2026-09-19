using CommunityToolkit.Mvvm.DependencyInjection;
using EcoEnergyManagement.ViewModels.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EcoEnergyManagement.UI.DependencyInjection
{
    public static class DependencyInjectionContainerExtensions
    {
        public static void ConfigureContainer(this Ioc container)
            => container.ConfigureServices(
                new ServiceCollection()
                .RegisterViewModelServices()
                .BuildServiceProvider());
    }
}
