using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using SlimWifiConfig.Model;
using SlimWifiConfig.Service;

namespace SlimWifiConfig.View
{
    public partial class MainWindow : Window
    {
        private readonly Page BasicSetupPage;
        private readonly Page WiFiSetupPage;
        private readonly Page TCPIPSetupPage;
        private readonly Page DataLoggerPage;
        private readonly Page RemoteTerminalPage;
        private readonly Page SettingsPage;

        private SerialPort _SerialPort;
        private CommandProcessingService _CommandProcessor;
        private ModuleConfiguration _ModuleConfiguration;
        private ConfigurationReader _ConfigurationReader;
        private ConfigurationWriter _ConfigurationWriter;
        private LocalServerConfiguration _ServerConfiguration;

        public MainWindow()
        {
            InitializeComponent();

            _SerialPort = new SerialPort();
            _ModuleConfiguration = new ModuleConfiguration();
            _CommandProcessor = new CommandProcessingService(_SerialPort);         
            _ConfigurationReader = new ConfigurationReader(_CommandProcessor, _ModuleConfiguration);
            _ConfigurationWriter = new ConfigurationWriter(_CommandProcessor, _ModuleConfiguration);
            _ServerConfiguration = new LocalServerConfiguration();

            SettingsPage = new Settings(_SerialPort, _ConfigurationReader, _ConfigurationWriter, _ModuleConfiguration, _ServerConfiguration);
            BasicSetupPage = new BasicSetup(_CommandProcessor, _ModuleConfiguration);
            WiFiSetupPage = new WiFiSetup(_CommandProcessor, _ModuleConfiguration);
            TCPIPSetupPage = new TCPUDPSettings(_CommandProcessor, _ModuleConfiguration);
            DataLoggerPage = new DataLogging();
            RemoteTerminalPage = new RemoteTerminal(_ServerConfiguration);

            SettingsListViewItem.IsSelected = true;
        }

        private void BasicSetup_Selected(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("BasicSettings_Selected");
            CurrentView.Content = BasicSetupPage;
            ListViewItem selected = (ListViewItem)SettingsListView.SelectedItem;
            if (selected != null) selected.IsSelected = false;
        }

        private void WiFiSetup_Selected(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("WiFiSetup_Selected");
            CurrentView.Content = WiFiSetupPage;
            ListViewItem selected = (ListViewItem)SettingsListView.SelectedItem;
            if (selected != null) selected.IsSelected = false;
        }

        private void TCPUDPSettings_Selected(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("TCPUDPSettings_Selected");
            CurrentView.Content = TCPIPSetupPage;
            ListViewItem selected = (ListViewItem)SettingsListView.SelectedItem;
            if (selected != null) selected.IsSelected = false;
        }

        private void DataLogging_Selected(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("DataLogging_Selected");
            CurrentView.Content = DataLoggerPage;
            ListViewItem selected = (ListViewItem)SettingsListView.SelectedItem;
            if (selected != null) selected.IsSelected = false;
        }

        private void RemoteTerminal_Selected(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("RemoteTerminal_Selected");
            CurrentView.Content = RemoteTerminalPage;
            ListViewItem selected = (ListViewItem)SettingsListView.SelectedItem;
            if (selected != null) selected.IsSelected = false;
        }

        private void Settings_Selected(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Settings_Selected");
            CurrentView.Content = SettingsPage;
            ListViewItem selected = (ListViewItem)MainMenuListView.SelectedItem;
            if (selected != null) selected.IsSelected = false;
        }
    }
}
