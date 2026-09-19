using EcoEnergyManagement.Core.Windowing;
using System.Collections.Generic;

namespace EcoEnergyManagement.PlatformHelpers.Screens
{
    public interface IScreenListener
    {
        List<ScreenArea> Locations { get; }
    }
}
