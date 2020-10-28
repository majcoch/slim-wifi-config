using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SlimWifiConfig
{
    /// <summary>
    /// Interaction logic for WiFiSetup.xaml
    /// </summary>
    public partial class WiFiSetup : Page
    {
        public WiFiSetup()
        {
            InitializeComponent();
        }

        private void StationItem_Selected(object sender, RoutedEventArgs e)
        {
            StationModeSettingsStackPanel.Visibility = Visibility.Visible;
            SoftAPModeSettingsStackPanel.Visibility = Visibility.Collapsed;
        }

        private void SoftAPItem_Selected(object sender, RoutedEventArgs e)
        {
            StationModeSettingsStackPanel.Visibility = Visibility.Collapsed;
            SoftAPModeSettingsStackPanel.Visibility = Visibility.Visible;
        }

        private void StationAndSoftAPItem_Selected(object sender, RoutedEventArgs e)
        {
            StationModeSettingsStackPanel.Visibility = Visibility.Visible;
            SoftAPModeSettingsStackPanel.Visibility = Visibility.Visible;
        }
    }
}
