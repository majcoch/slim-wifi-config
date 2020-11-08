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

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            if (_ModuleConfiguration.ConfigurationValid)  // If configuration exists load aproperatie values on page
            {
                if (_ModuleConfiguration.Mode == ModuleMode.STATION)
                {
                    // Update page configuration
                    StationItem.Selected -= StationItem_Selected;
                    StationItem.IsSelected = true;
                    StationModeSettingsStackPanel.Visibility = Visibility.Visible;
                    SoftAPModeSettingsStackPanel.Visibility = Visibility.Collapsed;
                    StationItem.Selected += StationItem_Selected;
                    // Populate with data
                    LoadStationModeSettingOnPage();
                }
                else if (_ModuleConfiguration.Mode == ModuleMode.SOFT_AP)
                {
                    // Update page configuration
                    SoftAPItem.Selected -= SoftAPItem_Selected;
                    SoftAPItem.IsSelected = true;
                    StationModeSettingsStackPanel.Visibility = Visibility.Collapsed;
                    SoftAPModeSettingsStackPanel.Visibility = Visibility.Visible;
                    SoftAPItem.Selected += SoftAPItem_Selected;
                    // Populate with data
                    LoadAccessPointModeSettingOnPage();
                }
                else
                {
                    StationAndSoftAPItem.Selected -= StationItem_Selected;
                    StationAndSoftAPItem.IsSelected = true;
                    StationModeSettingsStackPanel.Visibility = Visibility.Visible;
                    SoftAPModeSettingsStackPanel.Visibility = Visibility.Visible;
                    StationAndSoftAPItem.Selected += StationItem_Selected;
                    // Populate with data
                    LoadStationModeSettingOnPage();
                    LoadAccessPointModeSettingOnPage();
                }
            }           
        }

        private void StationItem_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                _CommandProcessor.ExecuteCommand("AT+CWMODE=1", 100);
                _CommandProcessor.OnCommandSuccess += UpdateStationPanelExpanded;
            }
            catch (Exception)
            {
                Console.WriteLine("Serial port not connected");
            }
        }

        private void SoftAPItem_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                _CommandProcessor.ExecuteCommand("AT+CWMODE=2", 100);
                _CommandProcessor.OnCommandSuccess += UpdateAccessPointPanelExpanded;
            }
            catch (Exception)
            {
                Console.WriteLine("Serial port not connected");
            }           
        }

        private void StationAndSoftAPItem_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                _CommandProcessor.ExecuteCommand("AT+CWMODE=3", 100);
                _CommandProcessor.OnCommandSuccess += UpdateStationAndccessPointPanelExpanded;
            }
            catch (Exception)
            {
                Console.WriteLine("Serial port not connected");
            }
            
        }

        // Station mode 
        private void ScanAvaliableAPsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _CommandProcessor.ExecuteCommand("AT+CWLAP", 5000);
                _CommandProcessor.OnParseSuccessResponse += UpdateAvaliableAPs;
            }
            catch (Exception)
            {
                Console.WriteLine("Serial port not connected");
            }
        }

        private void DHCPEnabledRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {             
                _CommandProcessor.ExecuteCommand("AT+CWDHCP_DEF=1,1", 100);
                _CommandProcessor.OnCommandSuccess += UpdateStationDHCPEnabledSuccess;
                DHCPEnabledStackPanel.Visibility = Visibility.Visible;
                DHCPDisabledStackPanel.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                Console.WriteLine("Serial port not connected");
            }
        }

        private void RetriveStationIpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _CommandProcessor.ExecuteCommand("AT+CIPSTA?", 100);
                _CommandProcessor.OnParseSuccessResponse += UpdateStationIpInformation;
            }
            catch (Exception)
            {
                Console.WriteLine("Serial port not connected");
            }          
        }

        private void DHCPDisabledRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                _CommandProcessor.ExecuteCommand("AT+CWDHCP_DEF=1,0", 100);
                _CommandProcessor.OnCommandSuccess += UpdateStationDHCPDisabledSuccess;
                DHCPEnabledStackPanel.Visibility = Visibility.Collapsed;
                DHCPDisabledStackPanel.Visibility = Visibility.Visible;         
            }
            catch (Exception)
            {
                Console.WriteLine("Serial port not connected");
            }            
        }

        private void SetStationStaticDHCPButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string StaticIp = StationStaticIpTextBox.Text;
                string StaticGateway = StationStaticGatewayTextBox.Text;
                string StaticNetmask = StationStaticNetmaskTextBox.Text;

                _CommandProcessor.ExecuteCommand($"AT+CIPSTA=\"{StaticIp}\",\"{StaticGateway}\",\"{StaticNetmask}\"", 100);
                _CommandProcessor.OnCommandSuccess += UpdateStationStaticIpSuccess;
                _CommandProcessor.OnCommandSuccess += UpdateStationStaticIpError;
            }
            catch (Exception)
            {
                Console.WriteLine("Serial port not connected");
            }
        }

        private void ConnectToWiFiButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string NetworkName = AvaliableAPsComboBox.Text;
                string NetworkPassword = WiFiPasswordBox.Password;
                string Command = (bool)SaveWiFiConnectionToFlashCheckBox.IsChecked
                    ? $"AT+CWJAP_DEF=\"{NetworkName}\",\"{NetworkPassword}\""
                    : $"AT+CWJAP_CUR=\"{NetworkName}\",\"{NetworkPassword}\"";
                _CommandProcessor.ExecuteCommand(Command, 30000);
                AvaliableAPsComboBox.IsEnabled = false;
                WiFiPasswordBox.IsEnabled = false;
                _CommandProcessor.OnParseFailResponse += UpdateWiFiConnectionError;
                _CommandProcessor.OnCommandSuccess += UpdateWiFiConnectionStatus;
            }
            catch (Exception)
            {
                Console.WriteLine("Serial port not connected");
            }
        }

        private void StationMACRetriveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _CommandProcessor.ExecuteCommand("AT+CIPSTAMAC?", 100);
                _CommandProcessor.OnParseSuccessResponse += UpdateStationMac;
            }
            catch (Exception)
            {
                Console.WriteLine("Serial port not connected");
            }
        }
      

        // Access point
        private void AccesPointSaveConfigurtionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string AccessPointName = AccesPointNetworkNameTextBox.Text;
                string AccessPointPassword = AccesPointNetworkPasswordBox.Password;
                string AccessPointChannelId = AccesPointChannelIdTextBox.Text;
                string AccessPointAuthentication = AccesPointEncryptionMethodComboBox.SelectedIndex.ToString();

                _CommandProcessor.ExecuteCommand($"AT+CWSAP=\"{AccessPointName}\",\"{AccessPointPassword}\",{AccessPointChannelId},{AccessPointAuthentication}", 200);
                _CommandProcessor.OnCommandSuccess += UpdateAccessPointConfigurationSuccess;
            }
            catch (Exception)
            {
                Console.WriteLine("Serial port not connected");
            }
        }

        private void AccessPointAutoDHCPRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                _CommandProcessor.ExecuteCommand("AT+CWDHCPS_DEF=0", 100);
                _CommandProcessor.OnCommandSuccess += UpdateAccessPointAutoDHCPEnabledSuccess;
                AccessPointCustomDHCPStackPanel.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                Console.WriteLine("Serial port not connected");
            }
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

            try
            {
                _CommandProcessor.ExecuteCommand($"AT+CWDHCPS_DEF=1,{LeaseTime},\"{StartIp}\",\"{EndIp}\"", 100);
                _CommandProcessor.OnCommandSuccess += UpdateAccessPointAutoDHCPDisabledSuccess;
            }
            catch (Exception)
            {
                Console.WriteLine("Serial port not connected");
            }
        }

        private void RetriveConnectedDevicesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _CommandProcessor.ExecuteCommand("AT+CWLIF", 200);
                _CommandProcessor.OnParseSuccessResponse += UpdateConnectedDevicesList;
            }
            catch (Exception)
            {
                Console.WriteLine("Serial port not connected");
            }
        }

        private void AccessPointMACRetriveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _CommandProcessor.ExecuteCommand("AT+CIPSTAMAC?", 100);
                _CommandProcessor.OnParseSuccessResponse += UpdateAccessPointMAC;
            }
            catch (Exception)
            {
                Console.WriteLine("Serial port not connected");
            }
        }

        // Delegates
        private void UpdateStationPanelExpanded()
        {
            _ModuleConfiguration.Mode = ModuleMode.STATION;
            _ModuleConfiguration.StationConfiguration = new StationModeConfiguration();
            _ModuleConfiguration.AccessPointConfiguration = null;

            Dispatcher.Invoke(() =>
            {
                StationModeSettingsStackPanel.Visibility = Visibility.Visible;
                SoftAPModeSettingsStackPanel.Visibility = Visibility.Collapsed;
            });
            _CommandProcessor.OnCommandSuccess -= UpdateStationPanelExpanded;
        }

        private void UpdateAccessPointPanelExpanded()
        {
            _ModuleConfiguration.Mode = ModuleMode.SOFT_AP;
            _ModuleConfiguration.StationConfiguration = null;
            _ModuleConfiguration.AccessPointConfiguration = new AccessPointModeConfiguration();

            Dispatcher.Invoke(() =>
            {
                StationModeSettingsStackPanel.Visibility = Visibility.Collapsed;
                SoftAPModeSettingsStackPanel.Visibility = Visibility.Visible;
            });
            _CommandProcessor.OnCommandSuccess -= UpdateAccessPointPanelExpanded;
        }

        private void UpdateStationAndccessPointPanelExpanded()
        {
            _ModuleConfiguration.Mode = ModuleMode.STATION_AND_AP;
            _ModuleConfiguration.StationConfiguration = new StationModeConfiguration();
            _ModuleConfiguration.AccessPointConfiguration = new AccessPointModeConfiguration();

            Dispatcher.Invoke(() =>
            {
                StationModeSettingsStackPanel.Visibility = Visibility.Visible;
                SoftAPModeSettingsStackPanel.Visibility = Visibility.Visible;
            });
            _CommandProcessor.OnCommandSuccess -= UpdateStationAndccessPointPanelExpanded;
        }

        // Station configuration delegates
        private void UpdateAvaliableAPs(string CommandResponse)
        {
            string[] RawAvaliableAPs = CommandResponse.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> APNames = new List<string>();
            for (int i = 0; i < RawAvaliableAPs.Length; i++)
            {
                string[] splitted = RawAvaliableAPs[i].Split(',');
                string name = splitted[1].Replace("\"", "");
                if( name.Length > 0) APNames.Add(name);  // Add just the name of the AP
            }
            Dispatcher.Invoke(() =>
            {
                AvaliableAPsComboBox.Items.Clear();
                for (int i = 0; i < APNames.Count; i++) AvaliableAPsComboBox.Items.Add(APNames[i]);
            });
            _CommandProcessor.OnParseSuccessResponse -= UpdateAvaliableAPs;
        }

        private void UpdateStationDHCPEnabledSuccess()
        {
            _CommandProcessor.OnCommandSuccess -= UpdateStationDHCPEnabledSuccess;
            _ModuleConfiguration.StationConfiguration.DhcpSetting = true;
        }

        private void UpdateStationDHCPDisabledSuccess()
        {
            _CommandProcessor.OnCommandSuccess -= UpdateStationDHCPDisabledSuccess;
            _ModuleConfiguration.StationConfiguration.DhcpSetting = false;
        }

        private void UpdateStationStaticIpSuccess()
        {
            _CommandProcessor.OnCommandSuccess -= UpdateStationStaticIpSuccess;
            _CommandProcessor.OnCommandSuccess -= UpdateStationStaticIpError;

            IpConfiguration ipConfig = new IpConfiguration();
            Dispatcher.Invoke(() =>
            {
                ipConfig.ip = StationStaticIpTextBox.Text;
                ipConfig.gateway = StationStaticGatewayTextBox.Text;
                ipConfig.netmask = StationStaticNetmaskTextBox.Text;
            });
            
            _ModuleConfiguration.StationConfiguration.IpConfig = ipConfig;
        }
            // Display user error message
        private void UpdateStationStaticIpError()
        {
            _CommandProcessor.OnCommandSuccess -= UpdateStationStaticIpSuccess;
            _CommandProcessor.OnCommandSuccess -= UpdateStationStaticIpError;
        }

        private void UpdateStationMac(string CommandResponse)
        {
            string MAC = "N/A";
            try
            {
                MAC = CommandResponse.Substring(CommandResponse.IndexOf(":") + 1).Trim().Replace("\"", "").Replace(";", "");
                Dispatcher.Invoke(() =>
                {
                    StationMACTextBlock.Text = MAC;
                    StationMacParsingErrorTextBlock.Text = "";
                });
            }
            catch (Exception)
            {
                Dispatcher.Invoke(() =>
                {
                    StationMacParsingErrorTextBlock.Text = "Error parsing response. Try again";
                });
            }
            _CommandProcessor.OnParseSuccessResponse -= UpdateStationMac;
        }

        private void UpdateWiFiConnectionStatus()
        {
            _CommandProcessor.OnCommandSuccess -= UpdateWiFiConnectionStatus;
            _CommandProcessor.OnParseFailResponse -= UpdateWiFiConnectionError;
            
            Dispatcher.Invoke(() =>
            {
                _ModuleConfiguration.StationConfiguration.ConnectedNetworkName = AvaliableAPsComboBox.Text;
                ConnectionStatusTextBox.Text = AvaliableAPsComboBox.Text;
                AvaliableAPsComboBox.IsEnabled = true;
                WiFiPasswordBox.IsEnabled = true;
            });
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
                AvaliableAPsComboBox.IsEnabled = true;
                WiFiPasswordBox.IsEnabled = true;
            });

            _CommandProcessor.OnCommandSuccess -= UpdateWiFiConnectionStatus;
            _CommandProcessor.OnParseFailResponse -= UpdateWiFiConnectionError;
        }

        private void UpdateStationIpInformation(string CommandResponse)
        {
            try
            {
                string[] CommandLines = CommandResponse.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                string IpAddress = CommandLines[0].Split(':')[2].Replace("\"", "").Trim();
                string Gateway = CommandLines[1].Split(':')[2].Replace("\"", "").Trim();
                string Netmask = CommandLines[2].Split(':')[2].Replace("\"", "").Trim();
                Dispatcher.Invoke(() =>
                {
                    StationIpTextBlock.Text = IpAddress;
                    StationGatewayTextBlock.Text = Gateway;
                    StationNetmaskTextBlock.Text = Netmask;
                    ReadStationIpErrorTextBlock.Text = "";
                });
            }
            catch (Exception)
            {
                Dispatcher.Invoke(() =>
                {
                    ReadStationIpErrorTextBlock.Text = "Error parsing response. Try again";
                });
            }            
            _CommandProcessor.OnParseSuccessResponse -= UpdateStationIpInformation;
        }

        // Access point configuration delegates
        private void UpdateAccessPointConfigurationSuccess()
        {
            _CommandProcessor.OnCommandSuccess -= UpdateAccessPointConfigurationSuccess;
            Dispatcher.Invoke(() =>
            {
                _ModuleConfiguration.AccessPointConfiguration.Name =  AccesPointNetworkNameTextBox.Text;
                _ModuleConfiguration.AccessPointConfiguration.Password = AccesPointNetworkPasswordBox.Password;
                _ModuleConfiguration.AccessPointConfiguration.ChannelId = AccesPointChannelIdTextBox.Text;
                _ModuleConfiguration.AccessPointConfiguration.AuthMethod = (AccessPointAuthenticationMethod)AccesPointEncryptionMethodComboBox.SelectedIndex;
            });            
        }

        private void UpdateAccessPointAutoDHCPEnabledSuccess()
        {
            _ModuleConfiguration.AccessPointConfiguration.DhcpSetting = true;
            _CommandProcessor.OnCommandSuccess -= UpdateAccessPointAutoDHCPEnabledSuccess;
        }

        private void UpdateAccessPointAutoDHCPDisabledSuccess()
        {
            AccessPointDHCPConfiguration dhcConfig = new AccessPointDHCPConfiguration();
            Dispatcher.Invoke(() =>
            {
                dhcConfig.leaseTime = AccessPointDHCPLeaseTimeTextBox.Text;
                dhcConfig.startIp = AccessPointDHCPStartIpTextBox.Text;
                dhcConfig.endIp = AccessPointDHCPEndIpTextBox.Text;
            });
            _ModuleConfiguration.AccessPointConfiguration.DhcpSetting = false;
            _ModuleConfiguration.AccessPointConfiguration.DhcpConfig = dhcConfig;          
            _CommandProcessor.OnCommandSuccess += UpdateAccessPointAutoDHCPDisabledSuccess;
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
            string MAC = "N/A";
            try
            {
                MAC = CommandResponse.Substring(CommandResponse.IndexOf(":") + 1).Trim().Replace("\"", "").Replace(";", "");
                Dispatcher.Invoke(() =>
                {
                    AccessPointMACTextBlock.Text = MAC;
                    ReadAccessPointMacErrorTextBlock.Text = "";
                });
            }
            catch (Exception)
            {
                Dispatcher.Invoke(() =>
                {
                    ReadAccessPointMacErrorTextBlock.Text = "Error parsing response. Try again";
                });
            }
            _CommandProcessor.OnParseSuccessResponse -= UpdateAccessPointMAC;
        }


        private void LoadStationModeSettingOnPage()
        {        
            if(null != _ModuleConfiguration.StationConfiguration.ConnectedNetworkName)
            {
                ConnectionStatusTextBox.Text = _ModuleConfiguration.StationConfiguration.ConnectedNetworkName;
            }
            else
            {
                ConnectionStatusTextBox.Text = "Offline";
            }
            if (_ModuleConfiguration.StationConfiguration.DhcpSetting)
            {
                DHCPEnabledRadioButton.Checked -= DHCPEnabledRadioButton_Checked;
                DHCPEnabledRadioButton.IsChecked = true;
                DHCPEnabledStackPanel.Visibility = Visibility.Visible;
                DHCPEnabledRadioButton.Checked += DHCPEnabledRadioButton_Checked;
            }
            else
            {
                DHCPDisabledRadioButton.Checked -= DHCPDisabledRadioButton_Checked;
                DHCPDisabledRadioButton.IsChecked = true;
                DHCPDisabledStackPanel.Visibility = Visibility.Visible;
                StationStaticIpTextBox.Text = _ModuleConfiguration.StationConfiguration.IpConfig.ip;
                StationStaticGatewayTextBox.Text = _ModuleConfiguration.StationConfiguration.IpConfig.gateway;
                StationStaticNetmaskTextBox.Text = _ModuleConfiguration.StationConfiguration.IpConfig.netmask;
                DHCPDisabledRadioButton.Checked += DHCPDisabledRadioButton_Checked;
            }
        }

        private void LoadAccessPointModeSettingOnPage()
        {
            AccesPointNetworkNameTextBox.Text = _ModuleConfiguration.AccessPointConfiguration.Name;
            AccesPointNetworkPasswordBox.Password = _ModuleConfiguration.AccessPointConfiguration.Password;
            AccesPointChannelIdTextBox.Text = _ModuleConfiguration.AccessPointConfiguration.ChannelId;
            AccesPointEncryptionMethodComboBox.SelectedIndex = (int)_ModuleConfiguration.AccessPointConfiguration.AuthMethod;

            if (_ModuleConfiguration.AccessPointConfiguration.DhcpSetting)
            {
                AccessPointAutoDHCPRadioButton.Checked -= AccessPointAutoDHCPRadioButton_Checked;
                AccessPointAutoDHCPRadioButton.IsChecked = true;
                AccessPointAutoDHCPRadioButton.Checked += AccessPointAutoDHCPRadioButton_Checked;
            }
            else
            {
                AccessPointStaticDHCPRadioButton.Checked -= AccessPointStaticDHCPRadioButton_Checked;
                AccessPointStaticDHCPRadioButton.IsChecked = true;
                AccessPointDHCPLeaseTimeTextBox.Text = _ModuleConfiguration.AccessPointConfiguration.DhcpConfig.leaseTime;
                AccessPointDHCPStartIpTextBox.Text = _ModuleConfiguration.AccessPointConfiguration.DhcpConfig.startIp;
                AccessPointDHCPEndIpTextBox.Text = _ModuleConfiguration.AccessPointConfiguration.DhcpConfig.endIp;
                AccessPointCustomDHCPStackPanel.Visibility = Visibility.Visible;
                AccessPointStaticDHCPRadioButton.Checked += AccessPointStaticDHCPRadioButton_Checked;
            }
        }
    }
}