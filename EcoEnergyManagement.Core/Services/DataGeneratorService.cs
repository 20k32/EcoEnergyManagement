using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcoEnergyManagement.Core.Data;
using EcoEnergyManagement.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoEnergyManagement.Core.Services;

public class DataGeneratorService
{
    private readonly AppDbContext _db;
    private readonly Random _random = new();

    public DataGeneratorService(AppDbContext db)
    {
        _db = db;
    }

    public async Task GenerateAllAsync(
        int startYear = 2016,
        int endYear = 2025)
    {
        await GenerateConsumptionAsync(startYear, endYear);

        await GenerateServiceOutputAsync(
            startYear,
            endYear);

        await GenerateTemperatureAsync(
            startYear,
            endYear);
    }

    // =========================================
    // RESOURCE CONSUMPTION
    // =========================================

    public async Task GenerateConsumptionAsync(
        int startYear,
        int endYear)
    {
        var limits = await _db.ResourceLimits
            .AsNoTracking()
            .ToListAsync();

        var existing = await _db.ResourceConsumptions
            .Select(x => new
            {
                x.OrganizationUnitId,
                x.ResourceId,
                x.Year,
                x.Month
            })
            .ToHashSetAsync();

        foreach (var limit in limits)
        {
            for (var year = startYear; year <= endYear; year++)
            {
                for (var month = 1; month <= 12; month++)
                {
                    var key = new
                    {
                        OrganizationUnitId =
                            limit.OrganizationUnitId,

                        ResourceId =
                            limit.ResourceId,

                        Year = year,
                        Month = month
                    };

                    if (existing.Contains(key))
                        continue;

                    var value = NextDecimal(
                        limit.MinValue,
                        limit.MaxValue);

                    _db.ResourceConsumptions.Add(
                        new ResourceConsumption
                        {
                            OrganizationUnitId =
                                limit.OrganizationUnitId,

                            ResourceId =
                                limit.ResourceId,

                            Year = year,
                            Month = month,

                            Amount = Math.Round(
                                value,
                                2)
                        });
                }
            }
        }

        await _db.SaveChangesAsync();
    }

    // =========================================
    // SERVICE OUTPUT
    // =========================================

    public async Task GenerateServiceOutputAsync(
        int startYear,
        int endYear)
    {
        // Use the Education Center as
        // the main service-producing unit.

        var unit = await _db.OrganizationUnits
            .FirstAsync(x =>
                x.Name == "Навчальний центр");

        var existing = await _db.ServiceOutputs
            .Where(x =>
                x.OrganizationUnitId == unit.Id &&
                x.Year >= startYear &&
                x.Year <= endYear)
            .Select(x => new
            {
                x.Year,
                x.Month
            })
            .ToHashSetAsync();

        for (var year = startYear; year <= endYear; year++)
        {
            for (var month = 1; month <= 12; month++)
            {
                if (existing.Contains(new
                    {
                        Year = year,
                        Month = month
                    }))
                {
                    continue;
                }

                // Conditional educational data:
                // monthly service volume in UAH.

                var amount = NextDecimal(
                    50_000m,
                    150_000m);

                _db.ServiceOutputs.Add(
                    new ServiceOutput
                    {
                        OrganizationUnitId = unit.Id,
                        Year = year,
                        Month = month,
                        Amount = Math.Round(
                            amount,
                            2)
                    });
            }
        }

        await _db.SaveChangesAsync();
    }

    // =========================================
    // TEMPERATURE
    // =========================================

    public async Task GenerateTemperatureAsync(
        int startYear,
        int endYear)
    {
        var units = await _db.OrganizationUnits
            .Where(x => x.ParentId != null)
            .ToListAsync();

        var existing = await _db.TemperatureRecords
            .Select(x => new
            {
                x.OrganizationUnitId,
                x.Year,
                x.Month
            })
            .ToHashSetAsync();

        for (var year = startYear; year <= endYear; year++)
        {
            for (var month = 1; month <= 12; month++)
            {
                var baseTemperature =
                    GetAverageTemperature(month);

                foreach (var unit in units)
                {
                    var key = new
                    {
                        OrganizationUnitId = unit.Id,
                        Year = year,
                        Month = month
                    };

                    if (existing.Contains(key))
                        continue;

                    // Small random variation.
                    var temperature =
                        baseTemperature +
                        (decimal)(_random.NextDouble() * 4 - 2);

                    _db.TemperatureRecords.Add(
                        new TemperatureRecord
                        {
                            OrganizationUnitId =
                                unit.Id,

                            Year = year,
                            Month = month,

                            Temperature = Math.Round(
                                (decimal)temperature,
                                1)
                        });
                }
            }
        }

        await _db.SaveChangesAsync();
    }

    private decimal NextDecimal(
        decimal min,
        decimal max)
    {
        var value =
            min +
            (decimal)_random.NextDouble() *
            (max - min);

        return value;
    }

    private static decimal GetAverageTemperature(
        int month)
    {
        return month switch
        {
            1 => -2.4m,
            2 => -1.1m,
            3 => 4.5m,
            4 => 10.2m,
            5 => 16.4m,
            6 => 20.3m,
            7 => 22.1m,
            8 => 21.4m,
            9 => 15.8m,
            10 => 9.2m,
            11 => 3.1m,
            12 => -1.5m,
            _ => 0
        };
    }
}
