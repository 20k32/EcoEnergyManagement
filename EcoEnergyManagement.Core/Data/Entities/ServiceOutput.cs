namespace EcoEnergyManagement.Core.Data.Entities;

public class ServiceOutput
{
    public long Id { get; set; }

    public int OrganizationUnitId { get; set; }

    public OrganizationUnit OrganizationUnit { get; set; } = null!;

    public int Year { get; set; }

    public int Month { get; set; }

    // Обсяг наданих послуг, грн
    public decimal Amount { get; set; }
}
