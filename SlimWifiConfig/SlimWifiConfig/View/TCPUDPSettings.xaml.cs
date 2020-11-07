using SlimWifiConfig.Service;
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
