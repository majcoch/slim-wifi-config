using SlimWifiConfig.Service;
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

namespace SlimWifiConfig.View
{
    /// <summary>
    /// Interaction logic for TCPUDPSettings.xaml
    /// </summary>
    public partial class TCPUDPSettings : Page
    {
        private CommandProcessingService _CommandProcessor;

        public TCPUDPSettings()
        {
            InitializeComponent();
        }

        public TCPUDPSettings(CommandProcessingService Processor)
        {
            InitializeComponent();
            _CommandProcessor = Processor;
        }

        private void PingButton_Click(object sender, RoutedEventArgs e)
        {
            string Address = WebAddressIpTextBlock.Text;
            _CommandProcessor.ExecuteCommand($"AT+PING=\"{Address}\"", 10000);
            _CommandProcessor.OnParseSuccessResponse += UpdatePingInformation;


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
    }
}
