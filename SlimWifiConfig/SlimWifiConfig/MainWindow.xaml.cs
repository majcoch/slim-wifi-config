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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SmartSetup.IsSelected = true;
        }

        private void SmartSetup_Selected(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("SmartSetup_Selected");
            MainView.Content = new SmartSetup();
        }

        private void BasicSetup_Selected(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("BasicSettings_Selected");
            MainView.Content = new BasicSetup();
        }

        private void WiFiSetup_Selected(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("WiFiSetup_Selected");
            MainView.Content = new WiFiSetup();
        }

        private void TCPUDPSettings_Selected(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("TCPUDPSettings_Selected");
            MainView.Content = new TCPUDPSettings();
        }

        private void DataLogging_Selected(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("DataLogging_Selected");
            MainView.Content = new DataLogging();
        }

        private void Settings_Selected(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Settings_Selected");
            MainView.Content = new Settings();
        }
    }
}
