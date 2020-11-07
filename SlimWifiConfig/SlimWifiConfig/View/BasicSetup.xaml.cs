using SlimWifiConfig.Model;
using SlimWifiConfig.Service;
using System.Windows;
using System.Windows.Controls;

namespace SlimWifiConfig.View
{
    /// <summary>
    /// Interaction logic for BasicSetup.xaml
    /// </summary>
    public partial class BasicSetup : Page
    {
        private CommandProcessingService _CommandProcessor;

        private ModuleConfiguration _ModuleConfiguration;

        public BasicSetup()
        {
            InitializeComponent();
        }

        public BasicSetup(CommandProcessingService Processor, ModuleConfiguration ModuleConfiguration)
        {
            InitializeComponent();
            _CommandProcessor = Processor;
            _ModuleConfiguration = ModuleConfiguration;
        }

        private void TestConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            _CommandProcessor.ExecuteCommand("AT", 100);
            _CommandProcessor.OnCommandSuccess += UpdateConnectionStatusSuccess;
            _CommandProcessor.OnCommandTimeout += UpdateConnectionStatusFailure;
        }

        private void RetriveFirmwareInformation_Button_Click(object sender, RoutedEventArgs e)
        {
            _CommandProcessor.ExecuteCommand("AT+GMR", 100);
            _CommandProcessor.OnParseSuccessResponse += UpdateFirmwareInformation;
        }

        private void CommandEchoingEnabledRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            _CommandProcessor.ExecuteCommand("ATE1", 100);
        }

        private void CommandEchoingDisabledRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            _CommandProcessor.ExecuteCommand("ATE0", 100);
        }

        private void SoftwareResetButton_Click(object sender, RoutedEventArgs e)
        {
            _CommandProcessor.ExecuteCommand("AT+RST",300);
        }

        private void RestoreToDefaultSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            _CommandProcessor.ExecuteCommand("AT+RESTORE", 300);
        }

        // Delegates for UI updating

        private void UpdateConnectionStatusSuccess()
        {
            Dispatcher.Invoke(() =>
            {
                ConnectionStatusTextBlock.Text = "AT running";
            });
            _CommandProcessor.OnCommandSuccess -= UpdateConnectionStatusSuccess;
            _CommandProcessor.OnCommandTimeout -= UpdateConnectionStatusFailure;
        }

        private void UpdateConnectionStatusFailure()
        {
            Dispatcher.Invoke(() =>
            {
                ConnectionStatusTextBlock.Text = "AT not responded";
            });
            _CommandProcessor.OnCommandTimeout -= UpdateConnectionStatusFailure;
            _CommandProcessor.OnCommandSuccess -= UpdateConnectionStatusSuccess;
        }

        private void UpdateFirmwareInformation(string CommandResponse)
        {
            string[] RawFirmwareData = CommandResponse.Split(';');
            string ATVersion = RawFirmwareData[0].Substring(RawFirmwareData[0].IndexOf(":") + 1, RawFirmwareData[0].Length - RawFirmwareData[0].IndexOf(":") - 1);
            string SDKVersion = RawFirmwareData[1].Substring(RawFirmwareData[1].IndexOf(":") + 1, RawFirmwareData[1].Length - RawFirmwareData[1].IndexOf(":") - 1);
            string CompileTime = RawFirmwareData[2].Substring(RawFirmwareData[2].IndexOf(":") + 1, RawFirmwareData[2].Length - RawFirmwareData[2].IndexOf(":") - 1);
            string BinVrsion =RawFirmwareData[3].Substring(RawFirmwareData[3].IndexOf(":") + 1, RawFirmwareData[3].Length - RawFirmwareData[3].IndexOf(":") - 1);

            Dispatcher.Invoke(() =>
            {
                ATVersionTextBlock.Text = ATVersion;
                SDKVersionTextBlock.Text = SDKVersion;
                CompileTimeTextBlock.Text = CompileTime;
                BinVersionTextBlock.Text = BinVrsion;
            });
            _CommandProcessor.OnParseSuccessResponse -= UpdateFirmwareInformation;
        }

    }
}
