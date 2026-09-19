namespace EcoEnergyManagement.Core.Data.Entities;

public class TemperatureRecord
{
    public long Id { get; set; }

    public int OrganizationUnitId { get; set; }

    public OrganizationUnit OrganizationUnit { get; set; } = null!;

    public int Year { get; set; }

    public int Month { get; set; }

    public decimal Temperature { get; set; }
}
