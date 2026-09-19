using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace EcoEnergyManagement.UI
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            NavView.SelectedItem = NavView.MenuItems[0];
            ContentFrame.Navigate(typeof(HomePage));
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var tag = (args.InvokedItemContainer as NavigationViewItem)?.Tag as string;
            if (tag == null) return;

            System.Type pageType = tag switch
            {
                "home" => typeof(HomePage),
                "inventory" => typeof(InventoryPage),
                "dataentry" => typeof(DataEntryPage),
                "reports" => typeof(ReportsPage),
                _ => typeof(HomePage),
            };

            ContentFrame.Navigate(pageType);
        }
    }
}

