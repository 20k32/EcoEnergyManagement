using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcoEnergyManagement.Core.Data;
using EcoEnergyManagement.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace EcoEnergyManagement.ViewModels;

public partial class ReportViewModel : ObservableObject
{
    private readonly AppDbContext _db;
    private readonly TariffService _tariffService;
    private readonly ReportService _reportService;
    private readonly OrganizationViewModel _orgViewModel;

    [ObservableProperty]
    private int selectedYear = DateTime.Now.Year;

    [ObservableProperty]
    private int selectedUnitId;

    [ObservableProperty]
    private string selectedUnitName = string.Empty;

    [ObservableProperty]
    private ObservableCollection<int> availableYears =
        new();

    [ObservableProperty]
    private ObservableCollection<OrganizationUnitViewModel>
        allUnits = new();

    [ObservableProperty]
    private ObservableCollection<ReportRowViewModel>
        reportRows = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    public ReportViewModel()
    {
        _db = new AppDbContext();
        _tariffService = new TariffService(_db);
        _reportService =
            new ReportService(_db, _tariffService);
        _orgViewModel = new OrganizationViewModel();

        InitializeYears();
        LoadUnitsAsync();
    }

    private void InitializeYears()
    {
        AvailableYears.Clear();
        for (int year = 2016; year <= 2026; year++)
        {
            AvailableYears.Add(year);
        }
    }

    private async void LoadUnitsAsync()
    {
        await _orgViewModel.LoadOrganizationCommand
            .ExecuteAsync(null);

        AllUnits = _orgViewModel.AllUnits;
    }

    [RelayCommand]
    public async Task BuildReportAsync()
    {
        if (SelectedUnitId == 0)
        {
            ErrorMessage =
                "Please select a unit first";
            return;
        }

        try
        {
            IsLoading = true;
            ErrorMessage = null;

            // Get unit name
            var unit = await _db.OrganizationUnits
                .FirstAsync(x =>
                    x.Id == SelectedUnitId);

            SelectedUnitName = unit.Name;

            // Build report using ReportService
            var reportData =
                await _reportService
                    .BuildReportAsync(
                        SelectedUnitId,
                        SelectedYear);

            // Convert to ViewModel
            ReportRows.Clear();

            foreach (var row in reportData)
            {
                if (row is Dictionary<string, object>
                    dictRow)
                {
                    ReportRows.Add(
                        new ReportRowViewModel(dictRow));
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage =
                $"Failed to build report: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task BuildSummaryReportAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            SelectedUnitName = "SUMMARY - All Units";

            // Get all units with data for the year
            var units = await _db.OrganizationUnits
                .Where(x => x.ParentId != null)
                .AsNoTracking()
                .ToListAsync();

            // Aggregate data from all units
            var aggregatedRows =
                new List<Dictionary<string, object>>();

            var resources = await _db.Resources
                .AsNoTracking()
                .ToListAsync();

            foreach (var resource in resources)
            {
                // Consumption row
                var consumptionRow =
                    new Dictionary<string, object>
                    {
                        {
                            "Indicator",
                            $"Споживання {resource.Name}"
                        },
                        { "Unit", resource.Unit }
                    };

                var expenseRow =
                    new Dictionary<string, object>
                    {
                        {
                            "Indicator",
                            $"Витрати на {resource.Name}"
                        },
                        { "Unit", "грн" }
                    };

                // Aggregate across all units
                for (int month = 1; month <= 12; month++)
                {
                    var monthConsumptions = await _db
                        .ResourceConsumptions
                        .Where(x =>
                            x.ResourceId ==
                            resource.Id &&
                            x.Year == SelectedYear &&
                            x.Month == month)
                        .ToListAsync();

                    decimal totalConsumption =
                        monthConsumptions
                            .Sum(x => x.Amount);

                    SetMonthValue(
                        consumptionRow,
                        month,
                        totalConsumption);

                    // Calculate expenses
                    decimal totalExpense = 0;
                    foreach (var consumption in
                        monthConsumptions)
                    {
                        var expense = await _tariffService
                            .CalculateExpenseAsync(
                                resource.Id,
                                SelectedYear,
                                month,
                                consumption.Amount);

                        totalExpense += expense;
                    }

                    SetMonthValue(
                        expenseRow,
                        month,
                        totalExpense);
                }

                aggregatedRows.Add(consumptionRow);
                aggregatedRows.Add(expenseRow);
            }

            // Add temperature
            var tempRow =
                new Dictionary<string, object>
                {
                    { "Indicator", "Температура довкілля" },
                    { "Unit", "°C" }
                };

            for (int month = 1; month <= 12; month++)
            {
                var temps = await _db.TemperatureRecords
                    .Where(x =>
                        x.Year == SelectedYear &&
                        x.Month == month)
                    .AsNoTracking()
                    .ToListAsync();

                decimal avgTemp =
                    temps.Count > 0
                        ? temps
                            .Average(x => x.Temperature)
                        : 0;

                SetMonthValue(
                    tempRow,
                    month,
                    avgTemp);
            }

            aggregatedRows.Add(tempRow);

            // Add service output
            var outputRow =
                new Dictionary<string, object>
                {
                    {
                        "Indicator",
                        "Обсяг наданих послуг"
                    },
                    { "Unit", "грн" }
                };

            for (int month = 1; month <= 12; month++)
            {
                var outputs = await _db.ServiceOutputs
                    .Where(x =>
                        x.Year == SelectedYear &&
                        x.Month == month)
                    .AsNoTracking()
                    .ToListAsync();

                decimal totalOutput =
                    outputs.Sum(x => x.Amount);

                SetMonthValue(
                    outputRow,
                    month,
                    totalOutput);
            }

            aggregatedRows.Add(outputRow);

            // Convert to ViewModel
            ReportRows.Clear();
            foreach (var row in aggregatedRows)
            {
                ReportRows.Add(
                    new ReportRowViewModel(row));
            }
        }
        catch (Exception ex)
        {
            ErrorMessage =
                $"Failed to build summary: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
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

/// <summary>
/// ViewModel wrapper for report row data
/// from Dictionary to support binding
/// </summary>
public partial class ReportRowViewModel : ObservableObject
{
    private readonly Dictionary<string, object> _data;

    public string Indicator =>
        _data.ContainsKey("Indicator")
            ? _data["Indicator"]?.ToString() ??
                string.Empty
            : string.Empty;

    public string Unit =>
        _data.ContainsKey("Unit")
            ? _data["Unit"]?.ToString() ??
                string.Empty
            : string.Empty;

    public decimal January =>
        GetValue("January");

    public decimal February =>
        GetValue("February");

    public decimal March =>
        GetValue("March");

    public decimal April =>
        GetValue("April");

    public decimal May =>
        GetValue("May");

    public decimal June =>
        GetValue("June");

    public decimal July =>
        GetValue("July");

    public decimal August =>
        GetValue("August");

    public decimal September =>
        GetValue("September");

    public decimal October =>
        GetValue("October");

    public decimal November =>
        GetValue("November");

    public decimal December =>
        GetValue("December");

    public decimal Total =>
        January +
        February +
        March +
        April +
        May +
        June +
        July +
        August +
        September +
        October +
        November +
        December;

    public ReportRowViewModel(
        Dictionary<string, object> data)
    {
        _data = data;
    }

    private decimal GetValue(string month)
    {
        if (_data.ContainsKey(month))
        {
            var value = _data[month];
            return value is decimal d ? d : 0;
        }

        return 0;
    }
}
