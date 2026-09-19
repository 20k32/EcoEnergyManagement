using System.Collections.Generic;

namespace EcoEnergyManagement.Core.Data.Entities;

public class Resource
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public ICollection<Tariff> Tariffs { get; set; }
        = new List<Tariff>();

    public ICollection<ResourceConsumption> Consumptions { get; set; }
        = new List<ResourceConsumption>();

    public ICollection<ResourceLimit> Limits { get; set; }
        = new List<ResourceLimit>();
}
