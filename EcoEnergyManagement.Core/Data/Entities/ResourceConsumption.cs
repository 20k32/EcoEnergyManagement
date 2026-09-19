namespace EcoEnergyManagement.Core.Data.Entities;

public class ResourceConsumption
{
    public long Id { get; set; }

    public int OrganizationUnitId { get; set; }

    public OrganizationUnit OrganizationUnit { get; set; } = null!;

    public int ResourceId { get; set; }

    public Resource Resource { get; set; } = null!;

    public int Year { get; set; }

    public int Month { get; set; }

    public decimal Amount { get; set; }
}
