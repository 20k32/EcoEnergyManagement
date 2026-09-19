using System;
using System.Linq;
using System.Threading.Tasks;
using EcoEnergyManagement.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace EcoEnergyManagement.Core.Services;

public class TariffService
{
    private readonly AppDbContext _db;

    public TariffService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<decimal?> GetTariffAsync(
        int resourceId,
        int year,
        int month)
    {
        var monthStart =
            new DateTime(year, month, 1);

        var monthEnd =
            monthStart.AddMonths(1).AddTicks(-1);

        var tariff = await _db.Tariffs
            .Where(x =>
                x.ResourceId == resourceId &&

                // Tariff overlaps the requested month.
                x.StartDate <= monthEnd &&
                x.EndDate >= monthStart)

            .OrderByDescending(x => x.StartDate)
            .FirstOrDefaultAsync();

        return tariff?.Price;
    }

    public async Task<decimal> CalculateExpenseAsync(
        int resourceId,
        int year,
        int month,
        decimal consumption)
    {
        var tariff = await GetTariffAsync(
            resourceId,
            year,
            month);

        if (tariff is null)
            return 0;

        return Math.Round(
            consumption * tariff.Value,
            2);
    }
}
