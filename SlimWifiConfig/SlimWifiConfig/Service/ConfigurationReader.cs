using SlimWifiConfig.Model;
using System;

namespace SlimWifiConfig.Service
{
    public class ConfigurationReader
    {
        private ModuleConfiguration _moduleConfiguration;
        private CommandProcessingService _commandProcessingService;
        private bool commandParsed;

        public ConfigurationReader(CommandProcessingService commandProcessingService, ModuleConfiguration ModuleConfiguration)
        {
            _commandProcessingService = commandProcessingService;
            _moduleConfiguration = ModuleConfiguration;
            commandParsed = false;
        }

        public ModuleConfiguration GetModuleConfiguration()
        {
            
            _commandProcessingService.OnParseSuccessResponse += ParseModeSetting;
            commandParsed = false;
            _commandProcessingService.ExecuteCommand("AT+CWMODE?",200);          
            while (!commandParsed);
            _commandProcessingService.OnParseSuccessResponse -= ParseModeSetting;

            _commandProcessingService.OnParseSuccessResponse += ParseMultipleConnectionSetting;
            commandParsed = false;
            _commandProcessingService.ExecuteCommand("AT+CIPMUX?", 200);
            while (!commandParsed) ;
            _commandProcessingService.OnParseSuccessResponse -= ParseMultipleConnectionSetting;

            if (_moduleConfiguration.Mode == ModuleMode.STATION)
            {
                GetStationConfiguration();
            }
            else if (_moduleConfiguration.Mode == ModuleMode.SOFT_AP)
            {
                
                GetAccessPointConfiguration();
            }
            else
            {
                GetStationConfiguration();
                GetAccessPointConfiguration();
            }

            _moduleConfiguration.ConfigurationValid = true;
            return _moduleConfiguration;
        }

        /// Mode detection
        private void ParseModeSetting(string response)
        {
            if (response.Contains("1"))
            {
                _moduleConfiguration.Mode = ModuleMode.STATION;
            }
            else if (response.Contains("2"))
            {
                _moduleConfiguration.Mode = ModuleMode.SOFT_AP;
            }
            else if (response.Contains("3"))
            {
                _moduleConfiguration.Mode = ModuleMode.STATION_AND_AP;
            }
            commandParsed = true;
        }

        private void ParseMultipleConnectionSetting(string response)
        {
            if (response.Contains("1"))
            {
                _moduleConfiguration.MultipleConnections = true;
            }
            else
            {
                _moduleConfiguration.MultipleConnections = false;
            }
            commandParsed = true;
        }

        //// Station
        private void GetStationConfiguration()
        {
            _moduleConfiguration.StationConfiguration = new StationModeConfiguration();

            _commandProcessingService.OnParseSuccessResponse += ParseNetworkConnection;
            commandParsed = false;
            _commandProcessingService.ExecuteCommand("AT+CWJAP?", 200);
            while (!commandParsed) ;
            _commandProcessingService.OnParseSuccessResponse -= ParseNetworkConnection;

            _commandProcessingService.OnParseSuccessResponse += ParseStationDHCPSetting;
            commandParsed = false;
            _commandProcessingService.ExecuteCommand("AT+CWDHCP?", 200);
            while (!commandParsed) ;
            _commandProcessingService.OnParseSuccessResponse -= ParseStationDHCPSetting;

            _commandProcessingService.OnParseSuccessResponse += ParseStationIpConfig;
            commandParsed = false;
            _commandProcessingService.ExecuteCommand("AT+CIPSTA?", 200);
            while (!commandParsed) ;
            _commandProcessingService.OnParseSuccessResponse -= ParseStationIpConfig;
        }

        private void ParseNetworkConnection(string response)
        {
            if (response.Contains("No AP"))
            {
                _moduleConfiguration.StationConfiguration.ConnectedNetworkName = null;
            }
            else
            {
                string networkData = response.Replace("+CWJAP:", "");
                string[] networkParams = networkData.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                _moduleConfiguration.StationConfiguration.ConnectedNetworkName = networkParams[0].Replace("\"", "");
            }
            commandParsed = true;
        }

        private void ParseStationDHCPSetting(string response)
        {
            string dhcpSetting = response.Replace("+CWDHCP:", "");
            int dhcpValue = int.Parse(dhcpSetting);
            _moduleConfiguration.StationConfiguration.DhcpSetting = (dhcpValue & 0b10) == 0b10;
            commandParsed = true;
        }

        private void ParseStationIpConfig(string response)
        {
            IpConfiguration ipConfiguration = new IpConfiguration();
            string[] CommandLines = response.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            ipConfiguration.ip = CommandLines[0].Split(':')[2].Replace("\"", "").Trim();
            ipConfiguration.gateway = CommandLines[1].Split(':')[2].Replace("\"", "").Trim();
            ipConfiguration.netmask = CommandLines[2].Split(':')[2].Replace("\"", "").Trim();
            _moduleConfiguration.StationConfiguration.IpConfig = ipConfiguration;
            commandParsed = true;
        }

        //// Access Point
        private void GetAccessPointConfiguration()
        {
            _moduleConfiguration.AccessPointConfiguration = new AccessPointModeConfiguration();

            _commandProcessingService.OnParseSuccessResponse += ParseAccessPointConfiguration;
            commandParsed = false;
            _commandProcessingService.ExecuteCommand("AT+CWSAP?", 200);
            while (!commandParsed) ;
            _commandProcessingService.OnParseSuccessResponse -= ParseAccessPointConfiguration;

            _commandProcessingService.OnParseSuccessResponse += ParseAccessPointDHCPConfiguration;
            commandParsed = false;
            _commandProcessingService.ExecuteCommand("AT+CWDHCPS_DEF?", 200);
            while (!commandParsed) ;
            _commandProcessingService.OnParseSuccessResponse -= ParseAccessPointDHCPConfiguration;
        }

        private void ParseAccessPointConfiguration(string response)
        {
            string apConfig = response.Replace("+CWSAP:", "");
            string[] configParams = apConfig.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            _moduleConfiguration.AccessPointConfiguration.Name = configParams[0].Replace("\"", "");
            _moduleConfiguration.AccessPointConfiguration.Password = configParams[1].Replace("\"", "");
            _moduleConfiguration.AccessPointConfiguration.ChannelId = configParams[2];
            _moduleConfiguration.AccessPointConfiguration.AuthMethod = (AccessPointAuthenticationMethod)int.Parse(configParams[3]);
            commandParsed = true;
        }

        private void ParseAccessPointDHCPConfiguration(string response)
        {
            AccessPointDHCPConfiguration dhcpConfiguration = new AccessPointDHCPConfiguration();
            string dhcpData = response.Replace("+CWDHCPS_DEF:", "");
            string[] dhcpParams = dhcpData.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            dhcpConfiguration.leaseTime = dhcpParams[0];
            dhcpConfiguration.startIp = dhcpParams[1];
            dhcpConfiguration.endIp = dhcpParams[2];
            _moduleConfiguration.AccessPointConfiguration.DhcpConfig = dhcpConfiguration;
            _moduleConfiguration.AccessPointConfiguration.DhcpSetting = (dhcpConfiguration.leaseTime == "0");
            commandParsed = true;
        }
    }
}
