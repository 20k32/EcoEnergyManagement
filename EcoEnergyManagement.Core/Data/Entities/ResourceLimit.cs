namespace EcoEnergyManagement.Core.Data.Entities;

public class ResourceLimit
{
    public int Id { get; set; }

    public int OrganizationUnitId { get; set; }

    public OrganizationUnit OrganizationUnit { get; set; } = null!;

    public int ResourceId { get; set; }

    public Resource Resource { get; set; } = null!;

    public decimal MinValue { get; set; }

    public decimal MaxValue { get; set; }
}
