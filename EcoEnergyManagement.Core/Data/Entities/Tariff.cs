using System;

namespace EcoEnergyManagement.Core.Data.Entities;

public class Tariff
{
    public int Id { get; set; }

    public int ResourceId { get; set; }

    public Resource Resource { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal Price { get; set; }
}
