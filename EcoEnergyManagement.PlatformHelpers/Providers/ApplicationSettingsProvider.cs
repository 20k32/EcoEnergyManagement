using CommunityToolkit.Mvvm.DependencyInjection;
using EcoEnergyManagement.Core.Abstractions.Providers;
using EcoEnergyManagement.PlatformHelpers.Providers.ApplicationData;
using EcoEnergyManagement.PlatformHelpers.Windowing;
using System.Threading.Tasks;

namespace EcoEnergyManagement.PlatformHelpers.Providers
{
    sealed class ApplicationSettingsProvider : IApplicationSettingsProvider
    {
        readonly IWindowHelper _windowHelper;
        readonly IApplicationDataProvider _applicationDataProvider;

        public ApplicationSettingsProvider()
        {
            _windowHelper = Ioc.Default.GetRequiredService<IWindowHelper>();
            _applicationDataProvider = Ioc.Default.GetRequiredService<IApplicationDataProvider>();
        }

        public T GetSettingsValue<T>(string token)
        {
            var result = default(T);

            var settingsValue = _applicationDataProvider.GetSettingsValue(token);

            if (settingsValue is not null)
            {
                result = (T)settingsValue;
            }

            return result;
        }

        public async Task InitializeAsync(string defaultFileNameWithExtension)
        {
            await _windowHelper.WindowInitializationTask;
            await _applicationDataProvider.InitializeAsync(defaultFileNameWithExtension);
        }

        public void SetSettingsValue<T>(string key, T value)
        {
            _applicationDataProvider.SetSettingsValue(key, value);
        }
    }
}
