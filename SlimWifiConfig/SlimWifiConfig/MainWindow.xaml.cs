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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BasicSetupListViewItem.IsSelected = true;
        }

        private void BasicSetup_Selected(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("BasicSettings_Selected");
            CurrentView.Content = new BasicSetup();
            ListViewItem selected = (ListViewItem)SettingsListView.SelectedItem;
            if (selected != null) selected.IsSelected = false;
        }

        private void WiFiSetup_Selected(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("WiFiSetup_Selected");
            CurrentView.Content = new WiFiSetup();
            ListViewItem selected = (ListViewItem)SettingsListView.SelectedItem;
            if (selected != null) selected.IsSelected = false;
        }

        private void TCPUDPSettings_Selected(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("TCPUDPSettings_Selected");
            CurrentView.Content = new TCPUDPSettings();
            ListViewItem selected = (ListViewItem)SettingsListView.SelectedItem;
            if (selected != null) selected.IsSelected = false;
        }

        private void DataLogging_Selected(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("DataLogging_Selected");
            CurrentView.Content = new DataLogging();
            ListViewItem selected = (ListViewItem)SettingsListView.SelectedItem;
            if (selected != null) selected.IsSelected = false;
        }

        private void RemoteTerminal_Selected(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("RemoteTerminal_Selected");
            CurrentView.Content = new RemoteTerminal();
            ListViewItem selected = (ListViewItem)SettingsListView.SelectedItem;
            if (selected != null) selected.IsSelected = false;
        }

        private void Settings_Selected(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Settings_Selected");
            CurrentView.Content = new Settings();
            ListViewItem selected = (ListViewItem)MainMenuListView.SelectedItem;
            if (selected != null) selected.IsSelected = false;
        }
    }
}
