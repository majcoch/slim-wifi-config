using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
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
        private SerialPort Port;
        private CommandProcessingService CmdProcessor;
        private ConfigurationReader CfgReader;

        public MainWindow()
        {
            
            InitializeComponent();

            Port = new SerialPort();
            CmdProcessor = new CommandProcessingService(Port);
            CfgReader = new ConfigurationReader(CmdProcessor);

            SettingsPage = new Settings(Port, CfgReader);
            BasicSetupPage = new BasicSetup(CmdProcessor, CfgReader._moduleConfiguration);
            WiFiSetupPage = new WiFiSetup(CmdProcessor, CfgReader._moduleConfiguration);
            TCPIPSetupPage = new TCPUDPSettings(CmdProcessor);
            DataLoggerPage = new DataLogging();
            RemoteTerminalPage = new RemoteTerminal();

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
