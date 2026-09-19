using Microsoft.UI.Xaml;
using System.Threading.Tasks;

namespace EcoEnergyManagement.PlatformHelpers.Windowing
{
    public interface IWindowHelper
    {
        public Window MainWindow { get; set; }
        public void CenterMainWindow();
        public void ActivateApplicationWindow();

        Task WindowInitializationTask { get; }
    }
}
