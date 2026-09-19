using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using EcoEnergyManagement.Core.Data;
using EcoEnergyManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EcoEnergyManagement.UI
{
    public sealed partial class InventoryPage : Page
    {
        private readonly AppDbContext _db;
        private readonly OrganizationViewModel _viewModel;

        public InventoryPage()
        {
            this.InitializeComponent();
            _db = new AppDbContext();
            _viewModel = new OrganizationViewModel();
            this.DataContext = _viewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await LoadInventoryAsync();
        }

        private async Task LoadInventoryAsync()
        {
            try
            {
                await _viewModel.LoadOrganizationCommand.ExecuteAsync(null);
                RenderTreeView();
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Error loading inventory: {ex.Message}";
            }
        }

        private void RenderTreeView()
        {
            UnitsTreeView.ItemsSource = null;
            UnitsTreeView.ItemsSource = _viewModel.RootUnits;
        }

        private async void TreeView_ItemInvoked(TreeViewItem sender, TreeViewItemInvokedEventArgs args)
        {
            if (args.InvokedItem is OrganizationUnitViewModel unit)
            {
                await ShowUnitDetails(unit);
            }
        }

        private async Task ShowUnitDetails(OrganizationUnitViewModel unit)
        {
            UnitNameText.Text = unit.Name ?? "-";
            UnitTypeText.Text = unit.Type ?? "-";

            // Load resources for this unit
            var resources = await _viewModel.GetResourcesForUnitAsync(unit.Id);
            ResourcesListView.ItemsSource = resources.Select(r => new { Name = r.Name, Unit = r.Unit }).ToList();
        }
    }
}
