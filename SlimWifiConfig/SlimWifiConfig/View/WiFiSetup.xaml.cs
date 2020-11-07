using SlimWifiConfig.Model;
using SlimWifiConfig.Service;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SlimWifiConfig.View
{
    /// <summary>
    /// Interaction logic for WiFiSetup.xaml
    /// </summary>
    public partial class WiFiSetup : Page
    {
        private CommandProcessingService _CommandProcessor;

        private ModuleConfiguration _ModuleConfiguration;

        public WiFiSetup()
        {
            InitializeComponent();
        }

        public WiFiSetup(CommandProcessingService Processor, ModuleConfiguration ModuleConfiguration)
        {
            InitializeComponent();
            _CommandProcessor = Processor;
            _ModuleConfiguration = ModuleConfiguration;
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("WiFi Settings Page Loaded");
            if (_ModuleConfiguration._mode == ModuleMode.STATION)
            {
                StationItem.Selected -= StationItem_Selected;
                StationItem.IsSelected = true;
                StationModeSettingsStackPanel.Visibility = Visibility.Visible;
                StationItem.Selected += StationItem_Selected;
            }
            else if (_ModuleConfiguration._mode == ModuleMode.SOFT_AP)
            {
                SoftAPItem.Selected -= SoftAPItem_Selected;
                SoftAPItem.IsSelected = true;
                SoftAPModeSettingsStackPanel.Visibility = Visibility.Visible;
                SoftAPItem.Selected += SoftAPItem_Selected;
            }
            else
            {
                StationAndSoftAPItem.Selected -= StationItem_Selected;
                StationAndSoftAPItem.IsSelected = true;
                StationModeSettingsStackPanel.Visibility = Visibility.Visible;
                SoftAPModeSettingsStackPanel.Visibility = Visibility.Visible;
                StationAndSoftAPItem.Selected += StationItem_Selected;
            }
        }

        private void StationItem_Selected(object sender, RoutedEventArgs e)
        {          
            _CommandProcessor.ExecuteCommand("AT+CWMODE=1", 100);
            _CommandProcessor.OnCommandSuccess += UpdateStationPanelExpanded;
        }

        private void SoftAPItem_Selected(object sender, RoutedEventArgs e)
        {
            _CommandProcessor.ExecuteCommand("AT+CWMODE=2", 100);
            _CommandProcessor.OnCommandSuccess += UpdateAccessPointPanelExpanded;
        }

        private void StationAndSoftAPItem_Selected(object sender, RoutedEventArgs e)
        {
            _CommandProcessor.ExecuteCommand("AT+CWMODE=3", 100);
            _CommandProcessor.OnCommandSuccess += UpdateStationAndccessPointPanelExpanded;
        }

        private void ScanAvaliableAPsButton_Click(object sender, RoutedEventArgs e)
        {
            _CommandProcessor.ExecuteCommand("AT+CWLAP", 5000);
            _CommandProcessor.OnParseSuccessResponse += UpdateAvaliableAPs;
        }

        private void DHCPEnabledRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!DHCPEnabledStackPanel.IsVisible)
            {
                DHCPEnabledStackPanel.Visibility = Visibility.Visible;
                DHCPDisabledStackPanel.Visibility = Visibility.Collapsed;

                _CommandProcessor.ExecuteCommand("AT+CWDHCP_DEF=1,1", 100);
            }
        }

        private void RetriveStationIpButton_Click(object sender, RoutedEventArgs e)
        {
            _CommandProcessor.ExecuteCommand("AT+CIPSTA?", 100);
            _CommandProcessor.OnParseSuccessResponse += UpdateStationIpInformation;
        }

        private void DHCPDisabledRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!DHCPDisabledStackPanel.IsVisible)
            {
                DHCPEnabledStackPanel.Visibility = Visibility.Collapsed;
                DHCPDisabledStackPanel.Visibility = Visibility.Visible;

                _CommandProcessor.ExecuteCommand("AT+CWDHCP_DEF=1,0", 100);
            }
        }

        private void ConnectToWiFiButton_Click(object sender, RoutedEventArgs e)
        {
            string NetworkName = AvaliableAPsComboBox.Text;
            string NetworkPassword = WiFiPasswordBox.Password;
            string Command = (bool)SaveWiFiConnectionToFlashCheckBox.IsChecked
                ? $"AT+CWJAP_DEF=\"{NetworkName}\",\"{NetworkPassword}\""
                : $"AT+CWJAP_CUR=\"{NetworkName}\",\"{NetworkPassword}\"";
            _CommandProcessor.ExecuteCommand(Command, 30000);
            //_CommandProcessor.OnParseSuccessResponse += UpdateWiFiConnectionStatus;
            _CommandProcessor.OnParseFailResponse += UpdateWiFiConnectionError;
        }

        private void StationMACRetriveButton_Click(object sender, RoutedEventArgs e)
        {
            _CommandProcessor.ExecuteCommand("AT+CIPSTAMAC?", 100);
            _CommandProcessor.OnParseSuccessResponse += UpdateStationMac;
        }

        private void SetStationStaticDHCPButton_Click(object sender, RoutedEventArgs e)
        {
            string StaticIp = StationStaticIpTextBox.Text;
            string StaticGateway = StationStaticGatewayTextBox.Text;
            string StaticNetmask = StationStaticNetmaskTextBox.Text;

            _CommandProcessor.ExecuteCommand($"AT+CIPSTA=\"{StaticIp}\",\"{StaticGateway}\",\"{StaticNetmask}\"", 100);
            // This can return error when set values are incorrect!
        }

        private void AccesPointSaveConfigurtionButton_Click(object sender, RoutedEventArgs e)
        {
            string AccessPointName = AccesPointNetworkNameTextBox.Text;
            string AccessPointPassword = AccesPointNetworkPasswordBox.Password;
            string AccessPointChannelId = AccesPointChannelIdTextBox.Text;
            string AccessPointAuthentication = AccesPointEncryptionMethodComboBox.SelectedIndex.ToString();

            _CommandProcessor.ExecuteCommand($"AT+CWSAP=\"{AccessPointName}\",\"{AccessPointPassword}\",{AccessPointChannelId},{AccessPointAuthentication}", 200);
        }

        private void AccessPointAutoDHCPRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            _CommandProcessor.ExecuteCommand("AT+CWDHCPS_DEF=0", 100);
            AccessPointCustomDHCPStackPanel.Visibility = Visibility.Collapsed;
        }

        private void AccessPointStaticDHCPRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            AccessPointCustomDHCPStackPanel.Visibility = Visibility.Visible;
        }

        private void SetAccessPointStaticDHCPButton_Click(object sender, RoutedEventArgs e)
        {
            string LeaseTime = AccessPointDHCPLeaseTimeTextBox.Text;
            string StartIp = AccessPointDHCPStartIpTextBox.Text;
            string EndIp = AccessPointDHCPEndIpTextBox.Text;

            _CommandProcessor.ExecuteCommand($"AT+CWDHCPS_DEF=1,{LeaseTime},\"{StartIp}\",\"{EndIp}\"", 100);
            // This can return error when set values are incorrect!
        }

        private void RetriveConnectedDevicesButton_Click(object sender, RoutedEventArgs e)
        {
            _CommandProcessor.ExecuteCommand("AT+CWLIF", 200);
            _CommandProcessor.OnParseSuccessResponse += UpdateConnectedDevicesList;
        }

        private void AccessPointMACRetriveButton_Click(object sender, RoutedEventArgs e)
        {
            _CommandProcessor.ExecuteCommand("AT+CIPSTAMAC?", 100);
            _CommandProcessor.OnParseSuccessResponse += UpdateAccessPointMAC;
        }

        private void UpdateAvaliableAPs(string CommandResponse)
        {
            string[] RawAvaliableAPs = CommandResponse.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> APNames = new List<string>();
            for (int i = 0; i < RawAvaliableAPs.Length; i++)
            {
                string[] splitted = RawAvaliableAPs[i].Split(',');
                APNames.Add(splitted[1].Replace("\"", ""));  // Add just the name of the AP
            }
            Dispatcher.Invoke(() =>
            {
                AvaliableAPsComboBox.Items.Clear();
                for (int i = 0; i < APNames.Count; i++) AvaliableAPsComboBox.Items.Add(APNames[i]);
            });
            _CommandProcessor.OnParseSuccessResponse -= UpdateAvaliableAPs;
        }

        private void UpdateStationMac(string CommandResponse)
        {
            string MAC = CommandResponse.Substring(CommandResponse.IndexOf(":") + 1).Trim().Replace("\"", "").Replace(";", "");
            Dispatcher.Invoke(() =>
            {
                StationMACTextBlock.Text = MAC;
            });

            _CommandProcessor.OnParseSuccessResponse -= UpdateStationMac;
        }

        private void UpdateWiFiConnectionError(string CommandResponse)
        {
            string ErrorMassage = "";
            if (CommandResponse.Contains(":1"))
            {
                ErrorMassage = "Connection timeout";
            }
            else if (CommandResponse.Contains(":2"))
            {
                ErrorMassage = "Invalid password";
            }
            else if (CommandResponse.Contains(":3"))
            {
                ErrorMassage = "WiFi network unavaliable";
            }
            else if (CommandResponse.Contains(":4"))
            {
                ErrorMassage = "Connection failes";
            }
            else
            {
                ErrorMassage = "Unknown error";
            }
            Dispatcher.Invoke(() =>
            {
                ConnectionStatusTextBlock.Text = ErrorMassage;
            });

            //_CommandProcessor.OnParseSuccessResponse -= UpdateWiFiConnectionStatus;
            _CommandProcessor.OnParseFailResponse -= UpdateWiFiConnectionError;
        }

        private void UpdateStationIpInformation(string CommandResponse)
        {
            string[] CommandLines = CommandResponse.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string IpAddress = CommandLines[0].Split(':')[2].Replace("\"","").Trim();
            string Gateway = CommandLines[1].Split(':')[2].Replace("\"", "").Trim();
            string Netmask = CommandLines[2].Split(':')[2].Replace("\"", "").Trim();

            Dispatcher.Invoke(() =>
            {
                StationIpTextBlock.Text = IpAddress;
                StationGatewayTextBlock.Text = Gateway;
                StationNetmaskTextBlock.Text = Netmask;
            });
            _CommandProcessor.OnParseSuccessResponse -= UpdateStationIpInformation;
        }

        private void UpdateStationPanelExpanded()
        {
            Dispatcher.Invoke(() =>
            {
                StationModeSettingsStackPanel.Visibility = Visibility.Visible;
                SoftAPModeSettingsStackPanel.Visibility = Visibility.Collapsed;
            });           
            _CommandProcessor.OnCommandSuccess -= UpdateStationPanelExpanded;
        }

        private void UpdateAccessPointPanelExpanded()
        {
            Dispatcher.Invoke(() =>
            {
                StationModeSettingsStackPanel.Visibility = Visibility.Collapsed;
                SoftAPModeSettingsStackPanel.Visibility = Visibility.Visible;
            });            
            _CommandProcessor.OnCommandSuccess -= UpdateAccessPointPanelExpanded;
        }

        private void UpdateStationAndccessPointPanelExpanded()
        {
            Dispatcher.Invoke(() =>
            {
                StationModeSettingsStackPanel.Visibility = Visibility.Visible;
                SoftAPModeSettingsStackPanel.Visibility = Visibility.Visible;
            });         
            _CommandProcessor.OnCommandSuccess -= UpdateStationAndccessPointPanelExpanded;
        }

        private void UpdateConnectedDevicesList(string CommandResponse)
        {
            string[] ConnectedDevices = CommandResponse.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> ConnectedIPs = new List<string>();
            for(int i = 0; i < ConnectedDevices.Length; i++)
            {
                string[] DeviceIp = ConnectedDevices[i].Split(',');
                ConnectedIPs.Add(DeviceIp[0]);
            }
            Dispatcher.Invoke(() =>
            {
                ConnectedDevicesListView.Items.Clear();
                for (int i = 0; i < ConnectedIPs.Count; i++) ConnectedDevicesListView.Items.Add(ConnectedIPs[i]);
            });
            _CommandProcessor.OnParseSuccessResponse += UpdateConnectedDevicesList;
        }

        private void UpdateAccessPointMAC(string CommandResponse)
        {
            string MAC = CommandResponse.Substring(CommandResponse.IndexOf(":") + 1).Trim().Replace("\"", "").Replace(";", "");
            Dispatcher.Invoke(() =>
            {
                AccessPointMACTextBlock.Text = MAC;
            });

            _CommandProcessor.OnParseSuccessResponse -= UpdateAccessPointMAC;
        }
    }
}
