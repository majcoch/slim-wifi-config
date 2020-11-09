using System.Windows.Controls;
using System.Windows.Navigation;
using System.Diagnostics;
using System;
using System.IO.Ports;
using System.Windows;
using SlimWifiConfig.Service;
using SlimWifiConfig.Model;

namespace SlimWifiConfig.View
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        private SerialPort _SerialPort;
        private ModuleConfiguration _ModuleConfiguration;
        private ConfigurationReader _ConfigurationReader;
        private ConfigurationWriter _ConfigurationWriter;
        private LocalServerConfiguration _ServerConfiguration;

        public Settings(SerialPort Port, ConfigurationReader ConfigurationReader, ConfigurationWriter ConfigurationWriter, ModuleConfiguration ModuleConfiguration, LocalServerConfiguration ServerConfiguration)
        {
            InitializeComponent();
            _SerialPort = Port;
            _ModuleConfiguration = ModuleConfiguration;
            _ConfigurationReader = ConfigurationReader;
            _ConfigurationWriter = ConfigurationWriter;
            _ServerConfiguration = ServerConfiguration;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void ComPortsComboBox_DropDownOpened(object sender, EventArgs e)
        {
            string[] AvaliableCommPorts = SerialPort.GetPortNames();
            ComPortsComboBox.ItemsSource = AvaliableCommPorts;
        }

        private void ConnectSerialPortButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SerialPortErrorTextBlock.Text = "";     // Clear error message if exists
            if ( _SerialPort.IsOpen )
            {
                _SerialPort.Close();
                ConnectSerialPortButton.Content = "Connect";                
                ComPortsComboBox.IsEnabled = true;
                BaudRatesComboBox.IsEnabled = true;
                FrameFormatComboBox.IsEnabled = true;
            }
            else
            {
                try
                {
                    _SerialPort.PortName = ComPortsComboBox.Text;
                    _SerialPort.BaudRate = int.Parse(BaudRatesComboBox.Text);
                    string FrameFormat = FrameFormatComboBox.Text;
                    if (FrameFormat == "8N1")
                    {
                        _SerialPort.Parity = Parity.None;
                        _SerialPort.DataBits = 8;
                        _SerialPort.StopBits = StopBits.One;
                    }
                    else
                    {
                        _SerialPort.Parity = Parity.None;
                        _SerialPort.DataBits = 8;
                        _SerialPort.StopBits = StopBits.Two;
                    }
                    _SerialPort.ReadTimeout = 500;
                    _SerialPort.WriteTimeout = 500;
                    _SerialPort.Open();
                    ConnectSerialPortButton.Content = "Disconnect";
                    ComPortsComboBox.IsEnabled = false;
                    BaudRatesComboBox.IsEnabled = false;
                    FrameFormatComboBox.IsEnabled = false;
                }
                catch (UnauthorizedAccessException) {
                    SerialPortErrorTextBlock.Text = "Unable to open serial connection";
                }
                catch (ArgumentException)
                {
                    SerialPortErrorTextBlock.Text = "No serial port selected";
                }
            }

        }

        private void ReadCurrentConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            ReadConfigurationErrorTextBlock.Text = "";
            try
            {
                _ConfigurationReader.GetModuleConfiguration();
                if (!_ModuleConfiguration.ConfigurationValid)
                {
                    ReadConfigurationErrorTextBlock.Text = "Read invalid";
                }
            }
            catch (Exception)
            {
                ReadConfigurationErrorTextBlock.Text = "Unable to get module's configuration";
            }        
        }

        private void LoadConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ModuleConfiguration));
            System.IO.StreamReader file = new System.IO.StreamReader("esp8266.xml");
            ModuleConfiguration config = (ModuleConfiguration)reader.Deserialize(file);
            file.Close();
            
            if (config.ConfigurationValid)
            {
                _ModuleConfiguration.ConfigurationValid = config.ConfigurationValid;
                _ModuleConfiguration.Mode = config.Mode;
                _ModuleConfiguration.AccessPointConfiguration = config.AccessPointConfiguration;
                _ModuleConfiguration.StationConfiguration = config.StationConfiguration;
                _ConfigurationWriter.WriteModuleConfiguration();
            }
        }

        private void SaveCurrentConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            if (_ModuleConfiguration.ConfigurationValid)
            {
                System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(ModuleConfiguration));
                System.IO.FileStream file = System.IO.File.Create("esp8266.xml");
                writer.Serialize(file, _ModuleConfiguration);
                file.Close();
            }          
        }

        private void ApplyDefaultServerConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            string ServerAddress = DefaultServerAddressTextBox.Text;
            string ServerPort = DefaultServerPortNumberTextBox.Text;

            _ServerConfiguration.LocalAddress = ServerAddress;
            _ServerConfiguration.LocalPort = ServerPort;
        }
    }
}
