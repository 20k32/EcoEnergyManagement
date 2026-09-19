using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcoEnergyManagement.Core.Data;
using EcoEnergyManagement.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace EcoEnergyManagement.ViewModels;

public partial class DataManagementViewModel : ObservableObject
{
    private readonly AppDbContext _db;
    private readonly DataGeneratorService _generatorService;
    private readonly OrganizationViewModel _orgViewModel;

    [ObservableProperty]
    private int selectedYear = DateTime.Now.Year;

    [ObservableProperty]
    private int selectedUnitId;

    [ObservableProperty]
    private ObservableCollection<int> availableYears =
        new();

    [ObservableProperty]
    private ObservableCollection<OrganizationUnitViewModel>
        allUnits = new();

    [ObservableProperty]
    private bool isGenerating;

    [ObservableProperty]
    private string? statusMessage;

    [ObservableProperty]
    private string? errorMessage;

    public DataManagementViewModel()
    {
        _db = new AppDbContext();
        _generatorService = new DataGeneratorService(_db);
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
    public async Task GenerateConsumptionDataAsync()
    {
        if (SelectedUnitId == 0)
        {
            ErrorMessage = "Please select a unit first";
            return;
        }

        try
        {
            IsGenerating = true;
            ErrorMessage = null;
            StatusMessage = "Generating consumption data...";

            await _generatorService
                .GenerateConsumptionAsync(
                    SelectedYear,
                    SelectedYear);

            StatusMessage =
                $"Successfully generated consumption data for {SelectedYear}";
        }
        catch (Exception ex)
        {
            ErrorMessage =
                $"Failed to generate consumption data: {ex.Message}";
        }
        finally
        {
            IsGenerating = false;
        }
    }

    [RelayCommand]
    public async Task GenerateAllDataAsync()
    {
        try
        {
            IsGenerating = true;
            ErrorMessage = null;
            StatusMessage =
                "Generating consumption, temperature, and service output data...";

            await _generatorService.GenerateAllAsync(2016, 2026);

            StatusMessage =
                "Successfully generated all data for 2016-2026";
        }
        catch (Exception ex)
        {
            ErrorMessage =
                $"Failed to generate data: {ex.Message}";
        }
        finally
        {
            IsGenerating = false;
        }
    }

    [RelayCommand]
    public async Task GenerateServiceOutputAsync()
    {
        try
        {
            IsGenerating = true;
            ErrorMessage = null;
            StatusMessage = "Generating service output data...";

            await _generatorService
                .GenerateServiceOutputAsync(
                    SelectedYear,
                    SelectedYear);

            StatusMessage =
                $"Successfully generated service output for {SelectedYear}";
        }
        catch (Exception ex)
        {
            ErrorMessage =
                $"Failed to generate service output: {ex.Message}";
        }
        finally
        {
            IsGenerating = false;
        }
    }

    [RelayCommand]
    public async Task GenerateTemperatureAsync()
    {
        try
        {
            IsGenerating = true;
            ErrorMessage = null;
            StatusMessage = "Generating temperature data...";

            await _generatorService
                .GenerateTemperatureAsync(
                    SelectedYear,
                    SelectedYear);

            StatusMessage =
                $"Successfully generated temperature data for {SelectedYear}";
        }
        catch (Exception ex)
        {
            ErrorMessage =
                $"Failed to generate temperature: {ex.Message}";
        }
        finally
        {
            IsGenerating = false;
        }
    }

    public async Task<bool> UpdateConsumptionAsync(
        int unitId,
        int resourceId,
        int year,
        int month,
        decimal newAmount)
    {
        try
        {
            var record = await _db.ResourceConsumptions
                .FirstOrDefaultAsync(x =>
                    x.OrganizationUnitId == unitId &&
                    x.ResourceId == resourceId &&
                    x.Year == year &&
                    x.Month == month);

            if (record == null)
            {
                var newRecord = new EcoEnergyManagement
                    .Core.Data.Entities
                    .ResourceConsumption
                {
                    OrganizationUnitId = unitId,
                    ResourceId = resourceId,
                    Year = year,
                    Month = month,
                    Amount = newAmount
                };

                _db.ResourceConsumptions.Add(newRecord);
            }
            else
            {
                record.Amount = newAmount;
            }

            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            ErrorMessage =
                $"Failed to update consumption: {ex.Message}";
            return false;
        }
    }

    public async Task<bool> UpdateServiceOutputAsync(
        int unitId,
        int year,
        int month,
        decimal newAmount)
    {
        try
        {
            var record = await _db.ServiceOutputs
                .FirstOrDefaultAsync(x =>
                    x.OrganizationUnitId == unitId &&
                    x.Year == year &&
                    x.Month == month);

            if (record == null)
            {
                var newRecord = new EcoEnergyManagement
                    .Core.Data.Entities.ServiceOutput
                {
                    OrganizationUnitId = unitId,
                    Year = year,
                    Month = month,
                    Amount = newAmount
                };

                _db.ServiceOutputs.Add(newRecord);
            }
            else
            {
                record.Amount = newAmount;
            }

            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            ErrorMessage =
                $"Failed to update service output: {ex.Message}";
            return false;
        }
    }
}
