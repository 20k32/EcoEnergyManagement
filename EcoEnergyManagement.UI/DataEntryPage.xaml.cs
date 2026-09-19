using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using EcoEnergyManagement.ViewModels;

namespace EcoEnergyManagement.UI
{
    public sealed partial class DataEntryPage : Page
    {
        private readonly DataManagementViewModel _viewModel;

        public DataEntryPage()
        {
            this.InitializeComponent();
            _viewModel = new DataManagementViewModel();
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

        private async void GenerateConsumption_Click(object sender, RoutedEventArgs e)
        {
            await ExecuteGeneratorCommand(() => _viewModel.GenerateConsumptionDataCommand.ExecuteAsync(null));
        }

        private async void GenerateTemperature_Click(object sender, RoutedEventArgs e)
        {
            ProgressRing.IsActive = true;
            StatusText.Text = "Generating temperature data...";
            try
            {
                await _viewModel.GenerateTemperatureCommand.ExecuteAsync(null);
                StatusText.Text = $"Temperature data generated for {_viewModel.SelectedYear}";
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Error: {ex.Message}";
            }
            finally
            {
                ProgressRing.IsActive = false;
            }
        }

        private async void GenerateServiceOutput_Click(object sender, RoutedEventArgs e)
        {
            ProgressRing.IsActive = true;
            StatusText.Text = "Generating service output data...";
            try
            {
                await _viewModel.GenerateServiceOutputCommand.ExecuteAsync(null);
                StatusText.Text = $"Service output generated for {_viewModel.SelectedYear}";
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Error: {ex.Message}";
            }
            finally
            {
                ProgressRing.IsActive = false;
            }
        }

        private async void GenerateAll_Click(object sender, RoutedEventArgs e)
        {
            ProgressRing.IsActive = true;
            StatusText.Text = "Generating all data (2016-2026)...";
            try
            {
                await _viewModel.GenerateAllDataCommand.ExecuteAsync(null);
                StatusText.Text = "All data generated successfully!";
                ErrorText.Text = "";
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Error: {ex.Message}";
            }
            finally
            {
                ProgressRing.IsActive = false;
            }
        }

        private async Task ExecuteGeneratorCommand(Func<Task> command)
        {
            ProgressRing.IsActive = true;
            ErrorText.Text = "";
            try
            {
                await command();
                StatusText.Text = $"Consumption data generated for {_viewModel.SelectedYear}";
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Error: {ex.Message}";
            }
            finally
            {
                ProgressRing.IsActive = false;
            }
        }
    }
}
