using SlimWifiConfig.Model;
using SlimWifiConfig.Service;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SlimWifiConfig.View
{
    /// <summary>
    /// Interaction logic for TCPUDPSettings.xaml
    /// </summary>
    public partial class TCPUDPSettings : Page
    {
        private CommandProcessingService _CommandProcessor;
        private ModuleConfiguration _ModuleConfiguration;

        public TCPUDPSettings()
        {
            InitializeComponent();
        }

        public TCPUDPSettings(CommandProcessingService Processor, Model.ModuleConfiguration ModuleConfiguration)
        {
            InitializeComponent();
            _CommandProcessor = Processor;
            _ModuleConfiguration = ModuleConfiguration;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            if (_ModuleConfiguration.ConfigurationValid)
            {
                if (_ModuleConfiguration.MultipleConnections)
                {
                    EnableMultipleConnectionsRadioButton.Checked -= EnableMultipleConnectionsRadioButton_Checked;
                    EnableMultipleConnectionsRadioButton.IsChecked = true;
                    EnableMultipleConnectionsRadioButton.Checked += EnableMultipleConnectionsRadioButton_Checked;
                }
                else
                {
                    DisableMultipleConnectionsRadioButton.Checked -= DisableMultipleConnectionsRadioButton_Checked;
                    DisableMultipleConnectionsRadioButton.IsChecked = true;
                    DisableMultipleConnectionsRadioButton.Checked += DisableMultipleConnectionsRadioButton_Checked;
                }

            }
        }

        private void PingButton_Click(object sender, RoutedEventArgs e)
        {
            PingeResultTextBox.Text = "";
            try
            {
                string Address = WebAddressIpTextBlock.Text;
                _CommandProcessor.ExecuteCommand($"AT+PING=\"{Address}\"", 10000);
                _CommandProcessor.OnParseSuccessResponse += UpdatePingInformation;
            }
            catch (Exception)
            {
                Console.WriteLine("Serial port not connected");
            }


        }

        private void EnableMultipleConnectionsRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            _CommandProcessor.ExecuteCommand($"AT+CIPMUX=1", 100);
            _CommandProcessor.OnCommandSuccess += UpdateEnableMultipleConnectionSuccess;
        }

        private void DisableMultipleConnectionsRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            _CommandProcessor.ExecuteCommand($"AT+CIPMUX=0", 100);
            _CommandProcessor.OnCommandSuccess += UpdateDisableMultipleConnectionSuccess;
        }

        private void UpdatePingInformation(string CommandResponse)
        {
            string PingValue = CommandResponse.Replace("+", "").Replace(";", "");
            Dispatcher.Invoke(() =>
            {
                PingeResultTextBox.Text = (PingValue + "ms");
            });
            _CommandProcessor.OnParseSuccessResponse -= UpdatePingInformation;
        }

        private void UpdateEnableMultipleConnectionSuccess()
        {
            _CommandProcessor.OnCommandSuccess -= UpdateEnableMultipleConnectionSuccess;
            _ModuleConfiguration.MultipleConnections = true;
        }

        private void UpdateDisableMultipleConnectionSuccess()
        {
            _CommandProcessor.OnCommandSuccess -= UpdateDisableMultipleConnectionSuccess;
            _ModuleConfiguration.MultipleConnections = false;
        }
    }
}
