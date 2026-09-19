using EcoEnergyManagement.Core.Abstractions.Providers;
using EcoEnergyManagement.PlatformHelpers.Providers;
using EcoEnergyManagement.PlatformHelpers.Providers.ApplicationData;
using EcoEnergyManagement.PlatformHelpers.Providers.ApplicationPackage;
using EcoEnergyManagement.PlatformHelpers.Providers.FutureAccessList;
using EcoEnergyManagement.PlatformHelpers.Providers.PackageRelatedProviders.ApplicationDataAccess;
using EcoEnergyManagement.PlatformHelpers.Providers.PackageRelatedProviders.FutureAccessList;
using EcoEnergyManagement.PlatformHelpers.Screens;
using EcoEnergyManagement.PlatformHelpers.Windowing;
using Microsoft.Extensions.DependencyInjection;

namespace EcoEnergyManagement.PlatformHelpers.DependencyInjection
{
    public static class DependencyInjectionContainerExtensions
    {
        public static IServiceCollection RegisterPlatformHelpers(this IServiceCollection services)
            => services.AddSingleton<IApplicationPackageChecker, ApplicationPackageChecker>()
                .AddTransient<UnpackagedApplicationDataProvider>()
                .AddSingleton<PackagedApplicationDataProvider>()
                .AddSingleton<IApplicationDataProvider, ApplicationDataProviderWrapper>()
                .AddSingleton<PackagedFutureAccessListProvider>()
                .AddSingleton<UnpackagedFutureAccessListProvider>()
                .AddSingleton<IFutureAccessListProvider, FutureAccessListProviderWrapper>()
                .AddTransient<IStorageItemProvider, StorageItemProvider>()
                .AddSingleton<IApplicationKeyProvider, ApplicationKeyProvider>()
                .AddSingleton<IApplicationSettingsProvider, ApplicationSettingsProvider>()
                .AddSingleton<IScreenListener, ScreenListener>()
                .AddSingleton<IWindowHelper, WindowHelper>();
    }
}
