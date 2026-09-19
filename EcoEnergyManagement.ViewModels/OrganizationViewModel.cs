using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcoEnergyManagement.Core.Data;
using EcoEnergyManagement.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoEnergyManagement.ViewModels;

public partial class OrganizationViewModel : ObservableObject
{
    private readonly AppDbContext _db;

    [ObservableProperty]
    private ObservableCollection<OrganizationUnitViewModel> rootUnits =
        new();

    [ObservableProperty]
    private ObservableCollection<OrganizationUnitViewModel> allUnits =
        new();

    [ObservableProperty]
    private OrganizationUnitViewModel? selectedUnit;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    public OrganizationViewModel()
    {
        _db = new AppDbContext();
    }

    [RelayCommand]
    public async Task LoadOrganizationAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            // Load root units with their hierarchy
            var roots = await _db.OrganizationUnits
                .Include(x => x.Children)
                .Where(x => x.ParentId == null)
                .AsNoTracking()
                .ToListAsync();

            RootUnits.Clear();
            foreach (var root in roots)
            {
                RootUnits.Add(
                    new OrganizationUnitViewModel(root));
            }

            // Load all units (for dropdown/selection)
            var all = await _db.OrganizationUnits
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();

            AllUnits.Clear();
            foreach (var unit in all)
            {
                AllUnits.Add(
                    new OrganizationUnitViewModel(unit));
            }
        }
        catch (Exception ex)
        {
            ErrorMessage =
                $"Failed to load organization: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void SelectUnit(OrganizationUnitViewModel? unit)
    {
        SelectedUnit = unit;
    }

    public async Task<List<Resource>> GetResourcesForUnitAsync(
        int unitId)
    {
        try
        {
            var resources = await _db.ResourceLimits
                .Where(x => x.OrganizationUnitId == unitId)
                .Select(x => x.Resource)
                .OrderBy(x => x.Name)
                .AsNoTracking()
                .ToListAsync();

            return resources;
        }
        catch (Exception ex)
        {
            ErrorMessage =
                $"Failed to load resources: {ex.Message}";
            return new List<Resource>();
        }
    }
}

/// <summary>
/// ViewModel wrapper for OrganizationUnit entity
/// to support hierarchical display
/// </summary>
public partial class OrganizationUnitViewModel : ObservableObject
{
    private readonly OrganizationUnit _entity;

    [ObservableProperty]
    private ObservableCollection<OrganizationUnitViewModel>
        children = new();

    public int Id => _entity.Id;
    public string Name => _entity.Name;
    public string Type => _entity.Type;
    public int? ParentId => _entity.ParentId;

    public OrganizationUnitViewModel(
        OrganizationUnit entity)
    {
        _entity = entity;

        if (entity.Children != null)
        {
            foreach (var child in entity.Children)
            {
                Children.Add(
                    new OrganizationUnitViewModel(child));
            }
        }
    }

    public override string ToString() => Name;
}
