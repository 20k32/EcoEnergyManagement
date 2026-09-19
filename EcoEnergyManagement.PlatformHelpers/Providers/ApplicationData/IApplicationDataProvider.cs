using System.Threading.Tasks;
using Windows.Storage;

namespace EcoEnergyManagement.PlatformHelpers.Providers.ApplicationData
{
    public interface IApplicationDataProvider
    {
        StorageFolder LocalFolder { get; }
        object GetSettingsValue(string key);
        void SetSettingsValue(string key, object value);
        Task InitializeAsync(string defaultFileNameWithExtension);
    }
}
