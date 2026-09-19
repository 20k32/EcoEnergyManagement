using CommunityToolkit.Mvvm.DependencyInjection;
using EcoEnergyManagement.Core.Abstractions.Providers;
using EcoEnergyManagement.PlatformHelpers.Windowing;
using System.Threading.Tasks;

namespace EcoEnergyManagement.PlatformHelpers.Providers
{
    sealed class ApplicationKeyProvider : IApplicationKeyProvider
    {
        const string APPLICATION_KEY = "ModelingAppKey";

        readonly IWindowHelper _windowHelper;

        public string DrawingSettingsTokenKey => APPLICATION_KEY;

        public ApplicationKeyProvider()
        {
            _windowHelper = Ioc.Default.GetService<IWindowHelper>();
        }

        public async Task InitializeAsync() => await _windowHelper.WindowInitializationTask;
    }
}
