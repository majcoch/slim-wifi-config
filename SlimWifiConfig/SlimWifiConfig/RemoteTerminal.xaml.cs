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
    /// Interaction logic for RemoteTerminal.xaml
    /// </summary>
    public partial class RemoteTerminal : Page
    {
        public RemoteTerminal()
        {
            InitializeComponent();
        }

        private void ServerCheckButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!ServerSettingsStackPanel.IsVisible)
            {
                ClientSettingsStackPanel.Visibility = Visibility.Collapsed;
                ServerSettingsStackPanel.Visibility = Visibility.Visible;
            }
            
        }

        private void ClientCheckButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!ClientSettingsStackPanel.IsVisible)
            {
                ClientSettingsStackPanel.Visibility = Visibility.Visible;
                ServerSettingsStackPanel.Visibility = Visibility.Collapsed;
            }
        }
    }
}
