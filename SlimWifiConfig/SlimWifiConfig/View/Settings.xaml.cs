using System.Windows.Controls;
using System.Windows.Navigation;
using System.Diagnostics;
using System;
using System.IO.Ports;
using System.Windows;
using System.Text;

namespace SlimWifiConfig.View
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        public SerialPort _SerialPort;

        public Settings(SerialPort Port)
        {
            InitializeComponent();
            _SerialPort = Port;
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
            if( _SerialPort.IsOpen )
            {
                _SerialPort.Close();
                ConnectSerialPortButton.Content = "Connect";
                ComPortsComboBox.IsEnabled = true;
                BaudRatesComboBox.IsEnabled = true;
                FrameFormatComboBox.IsEnabled = true;
            }
            else
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

                try
                {
                    _SerialPort.ReadTimeout = 500;
                    _SerialPort.WriteTimeout = 500;
                    _SerialPort.Open();
                    ConnectSerialPortButton.Content = "Disconnect";
                    ComPortsComboBox.IsEnabled = false;
                    BaudRatesComboBox.IsEnabled = false;
                    FrameFormatComboBox.IsEnabled = false;
                }
                catch (Exception) { }
            }

        }
    }
}
