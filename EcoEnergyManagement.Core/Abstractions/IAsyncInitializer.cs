using System.Threading.Tasks;

namespace EcoEnergyManagement.Core.Abstractions
{
    public interface IAsyncInitializer
    {
        Task InitializeAsync();
    }
}
