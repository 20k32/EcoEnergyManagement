using EcoEnergyManagement.PlatformHelpers.Providers.ApplicationData;

namespace EcoEnergyManagement.PlatformHelpers.Providers.PackageRelatedProviders.ApplicationDataAccess
{
    interface IPackageRelatedApplicationDataProvider : IApplicationDataProvider
    {
        void Remove(string key);
        bool ContainsKey(string key);
    }
}
