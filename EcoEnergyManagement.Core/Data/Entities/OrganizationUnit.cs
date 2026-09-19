using System.Collections.Generic;

namespace EcoEnergyManagement.Core.Data.Entities;

public class OrganizationUnit
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public int? ParentId { get; set; }

    public OrganizationUnit? Parent { get; set; }

    public ICollection<OrganizationUnit> Children { get; set; }
        = new List<OrganizationUnit>();

    public ICollection<ResourceConsumption> Consumptions { get; set; }
        = new List<ResourceConsumption>();

    public ICollection<ResourceLimit> ResourceLimits { get; set; }
        = new List<ResourceLimit>();

    public ICollection<ServiceOutput> ServiceOutputs { get; set; }
        = new List<ServiceOutput>();

    public ICollection<TemperatureRecord> TemperatureRecords { get; set; }
        = new List<TemperatureRecord>();
}
