using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using EcoEnergyManagement.ViewModels;
using EcoEnergyManagement.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace EcoEnergyManagement.UI
{
    public sealed partial class ReportsPage : Page
    {
        private readonly ReportViewModel _viewModel;
        private readonly AppDbContext _db;

        public ReportsPage()
        {
            this.InitializeComponent();
            _viewModel = new ReportViewModel();
            _db = new AppDbContext();
            this.DataContext = _viewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            InitializeControls();
        }

        private void InitializeControls()
        {
            // Bind years
            YearComboBox.ItemsSource = _viewModel.AvailableYears;
            if (_viewModel.AvailableYears.Count > 0)
            {
                YearComboBox.SelectedIndex = _viewModel.AvailableYears.IndexOf(_viewModel.SelectedYear);
            }

            // Bind units
            UnitComboBox.ItemsSource = _viewModel.AllUnits;
        }

        private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (YearComboBox.SelectedItem is int year)
            {
                _viewModel.SelectedYear = year;
            }
        }

        private void UnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UnitComboBox.SelectedItem is OrganizationUnitViewModel unit)
            {
                _viewModel.SelectedUnitId = unit.Id;
            }
        }

        private async void LoadReport_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedUnitId == 0)
            {
                ErrorText.Text = "Please select a structural unit first";
                return;
            }

            await LoadReportAsync();
        }

        private async void LoadSummary_Click(object sender, RoutedEventArgs e)
        {
            await LoadSummaryReportAsync();
        }

        private async Task LoadReportAsync()
        {
            ProgressRing.IsActive = true;
            ErrorText.Text = "";
            ReportTitleText.Text = "";

            try
            {
                await _viewModel.BuildReportCommand.ExecuteAsync(null);

                ReportTitleText.Text = $"Report for {_viewModel.SelectedUnitName} - {_viewModel.SelectedYear}";
                ReportDataGrid.ItemsSource = _viewModel.ReportRows;
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Error loading report: {ex.Message}";
            }
            finally
            {
                ProgressRing.IsActive = false;
            }
        }

        private async Task LoadSummaryReportAsync()
        {
            ProgressRing.IsActive = true;
            ErrorText.Text = "";
            ReportTitleText.Text = "";

            try
            {
                await _viewModel.BuildSummaryReportCommand.ExecuteAsync(null);
                ReportTitleText.Text = $"Summary Report for All Units - {_viewModel.SelectedYear}";
                ReportDataGrid.ItemsSource = _viewModel.ReportRows;
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Error loading summary report: {ex.Message}";
            }
            finally
            {
                ProgressRing.IsActive = false;
            }
        }
    }
}
