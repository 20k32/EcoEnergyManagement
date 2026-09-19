using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcoEnergyManagement.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace EcoEnergyManagement.Core.Services;

public class ReportService
{
    private readonly AppDbContext _db;
    private readonly TariffService _tariffService;

    public ReportService(
        AppDbContext db,
        TariffService tariffService)
    {
        _db = db;
        _tariffService = tariffService;
    }

    public async Task<List<dynamic>> BuildReportAsync(
        int organizationUnitId,
        int year)
    {
        var rows = new List<dynamic>();

        var consumptions =
            await _db.ResourceConsumptions
                .Include(x => x.Resource)
                .Where(x =>
                    x.OrganizationUnitId ==
                    organizationUnitId &&

                    x.Year == year)
                .ToListAsync();

        var resources =
            consumptions
                .Select(x => x.Resource)
                .DistinctBy(x => x.Id)
                .ToList();

        foreach (var resource in resources)
        {
            var consumptionRow =
                new Dictionary<string, object>
                {
                    { "Indicator", $"Споживання {resource.Name}" },
                    { "Unit", resource.Unit }
                };

            var expenseRow =
                new Dictionary<string, object>
                {
                    { "Indicator", $"Витрати на {resource.Name}" },
                    { "Unit", "грн" }
                };

            foreach (var consumption
                     in consumptions.Where(
                         x => x.ResourceId == resource.Id))
            {
                SetMonthValue(
                    consumptionRow,
                    consumption.Month,
                    consumption.Amount);

                var expense =
                    await _tariffService
                        .CalculateExpenseAsync(
                            consumption.ResourceId,
                            consumption.Year,
                            consumption.Month,
                            consumption.Amount);

                SetMonthValue(
                    expenseRow,
                    consumption.Month,
                    expense);
            }

            rows.Add(consumptionRow);
            rows.Add(expenseRow);
        }

        // Temperature
        var temperature =
            await _db.TemperatureRecords
                .Where(x =>
                    x.OrganizationUnitId ==
                    organizationUnitId &&

                    x.Year == year)
                .ToListAsync();

        var temperatureRow =
            new Dictionary<string, object>
            {
                { "Indicator", "Температура довкілля" },
                { "Unit", "°C" }
            };

        foreach (var record in temperature)
        {
            SetMonthValue(
                temperatureRow,
                record.Month,
                record.Temperature);
        }

        rows.Add(temperatureRow);

        // Service output
        var serviceOutput =
            await _db.ServiceOutputs
                .Where(x =>
                    x.OrganizationUnitId ==
                    organizationUnitId &&

                    x.Year == year)
                .ToListAsync();

        if (serviceOutput.Count > 0)
        {
            var outputRow =
                new Dictionary<string, object>
                {
                    { "Indicator", "Обсяг наданих послуг" },
                    { "Unit", "грн" }
                };

            foreach (var record in serviceOutput)
            {
                SetMonthValue(
                    outputRow,
                    record.Month,
                    record.Amount);
            }

            rows.Add(outputRow);
        }

        return rows;
    }

    private static void SetMonthValue(
        Dictionary<string, object> row,
        int month,
        decimal value)
    {
        var monthName = month switch
        {
            1 => "January",
            2 => "February",
            3 => "March",
            4 => "April",
            5 => "May",
            6 => "June",
            7 => "July",
            8 => "August",
            9 => "September",
            10 => "October",
            11 => "November",
            12 => "December",
            _ => string.Empty
        };

        if (!string.IsNullOrEmpty(monthName))
        {
            row[monthName] = value;
        }
    }
}
