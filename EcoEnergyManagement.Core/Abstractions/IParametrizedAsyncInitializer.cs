using System.Threading.Tasks;

namespace EcoEnergyManagement.Core.Abstractions
{
    public interface IParametrizedAsyncInitializer<T>
    {
        Task InitializeAsync(T parameter);
    }
}
