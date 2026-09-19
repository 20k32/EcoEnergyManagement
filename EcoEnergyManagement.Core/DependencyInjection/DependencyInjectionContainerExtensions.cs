using EcoEnergyManagement.Core.Dispatching;
using EcoEnergyManagement.Core.Logging;
using EcoEnergyManagement.Core.Logging.Formatting;
using EcoEnergyManagement.Core.Serializer;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace EcoEnergyManagement.Core.DependencyInjection
{
    public static class DependencyInjectionContainerExtensions
    {
        public static IServiceCollection RegisterCoreServices(this IServiceCollection services)
        {
            LoggerInitializer.Initialize();

            return services
                .AddSingleton<ISerializer, NewtonSoftSerializer>()
                .AddSingleton<IUserInterfaceThreadContext, UserInterfaceThreadContext>()
                .AddSingleton<ILogFormatter, LogFormatter>()
                .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(Log.Logger, dispose: true));
        }
    }
}
