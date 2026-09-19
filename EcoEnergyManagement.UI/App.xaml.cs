using CommunityToolkit.Mvvm.DependencyInjection;
using EcoEnergyManagement.Core.Dispatching;
using EcoEnergyManagement.Core.Logging;
using EcoEnergyManagement.PlatformHelpers.Windowing;
using EcoEnergyManagement.UI.DependencyInjection;
using Microsoft.UI.Xaml;
using System.Threading;

namespace EcoEnergyManagement.UI
{
    public partial class App : Application
    {
        static App()
        {
            Ioc.Default.ConfigureContainer();

            Ioc.Default.GetRequiredService<IUserInterfaceThreadContext>().Initialize(SynchronizationContext.Current);
        }

        public App()
        {
            InitializeComponent();

            Logger.Information("Application initialized");
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var windowHelper = Ioc.Default.GetRequiredService<IWindowHelper>();

            windowHelper.MainWindow = new MainWindow();

            windowHelper.CenterMainWindow();
            windowHelper.ActivateApplicationWindow();

            Logger.Information("Activated application window");
        }
    }
}
