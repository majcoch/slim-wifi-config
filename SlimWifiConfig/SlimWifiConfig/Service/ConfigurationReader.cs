using SlimWifiConfig.Model;
using System;

namespace SlimWifiConfig.Service
{
    public class ConfigurationReader
    {
        public ModuleConfiguration _moduleConfiguration;
        private CommandProcessingService _commandProcessingService;
        private bool commandParsed;

        public ConfigurationReader(CommandProcessingService commandProcessingService)
        {
            _commandProcessingService = commandProcessingService;
            _moduleConfiguration = new ModuleConfiguration();
            commandParsed = false;
        }

        public void GetModuleConfiguration()
        {
            _commandProcessingService.OnParseSuccessResponse += ParseModeSetting;
            commandParsed = false;
            _commandProcessingService.ExecuteCommand("AT+CWMODE?",200);          
            while (!commandParsed);
            _commandProcessingService.OnParseSuccessResponse -= ParseModeSetting;

            if(_moduleConfiguration._mode == ModuleMode.STATION)
            {
                GetStationConfiguration();
            }
            else if (_moduleConfiguration._mode == ModuleMode.SOFT_AP)
            {
                GetAccessPointConfiguration();
            }
            else
            {
                GetStationConfiguration();
                GetAccessPointConfiguration();
            }

        }

        public void ReadConfiguration(string FileName)
        {

        }

        public void WriteConfiguration(string FileName)
        {

        }

        
        private void ParseModeSetting(string response)
        {
            if (response.Contains("1"))
            {
                _moduleConfiguration._mode = ModuleMode.STATION;
            }
            else if (response.Contains("2"))
            {
                _moduleConfiguration._mode = ModuleMode.SOFT_AP;
            }
            else if (response.Contains("3"))
            {
                _moduleConfiguration._mode = ModuleMode.STATION_AND_AP;
            }
            commandParsed = true;
        }

        // Station
        private void GetStationConfiguration()
        {
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
            if(response.Contains("No AP"))
            {
                _moduleConfiguration._connectedNetworkName = "";
            }
            else
            {
                string networkData = response.Replace("+CWJAP:", "");
                string[] networkParams = networkData.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                _moduleConfiguration._connectedNetworkName = networkParams[0].Replace("\"", "");
            }      
            commandParsed = true;
        }

        private void ParseStationDHCPSetting(string response)
        {
            if (response.Contains("1"))
            {
                _moduleConfiguration._stationDHCPEnabled = true;
            }
            else
            {
                _moduleConfiguration._stationDHCPEnabled = false;
            }
            commandParsed = true;
        }

        private void ParseStationIpConfig(string response)
        {
            string[] CommandLines = response.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            _moduleConfiguration._stationIpConfig.ip = CommandLines[0].Split(':')[2].Replace("\"", "").Trim();
            _moduleConfiguration._stationIpConfig.gateway = CommandLines[1].Split(':')[2].Replace("\"", "").Trim();
            _moduleConfiguration._stationIpConfig.netmask = CommandLines[2].Split(':')[2].Replace("\"", "").Trim();
            commandParsed = true;
        }

        // Access Point

        private void GetAccessPointConfiguration()
        {
            _commandProcessingService.OnParseSuccessResponse += ParseAccessPointConfiguration;
            commandParsed = false;
            _commandProcessingService.ExecuteCommand("AT+CWSAP?", 200);
            while (!commandParsed) ;
            _commandProcessingService.OnParseSuccessResponse -= ParseAccessPointConfiguration;

            _commandProcessingService.OnParseSuccessResponse += ParseAccessPointDHCPSetting;
            commandParsed = false;
            _commandProcessingService.ExecuteCommand("AT+CWDHCP?", 300);
            while (!commandParsed) ;
            _commandProcessingService.OnParseSuccessResponse -= ParseAccessPointDHCPSetting;

            _commandProcessingService.OnParseSuccessResponse += ParseAccessPointDHCPConfiguration;
            commandParsed = false;
            _commandProcessingService.ExecuteCommand("AT+CWDHCPS_DEF?", 200);
            while (!commandParsed) ;
            _commandProcessingService.OnParseSuccessResponse -= ParseAccessPointDHCPConfiguration;

            _commandProcessingService.OnParseSuccessResponse += ParseAccessPointIpConfiguration;
            commandParsed = false;
            _commandProcessingService.ExecuteCommand("AT+CIPAP?", 200);
            while (!commandParsed) ;
            _commandProcessingService.OnParseSuccessResponse -= ParseAccessPointIpConfiguration;
        }

        private void ParseAccessPointConfiguration(string response)
        {
            string apConfig = response.Replace("+CWSAP:", "");
            string[] configParams = apConfig.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            _moduleConfiguration._hostName = configParams[0].Replace("\"", "");
            _moduleConfiguration._hostPassword = configParams[1].Replace("\"", "");
            commandParsed = true;
        }

        private void ParseAccessPointDHCPSetting(string response)
        {
            if (response.Contains("1"))
            {
                _moduleConfiguration._accessPointDHCPEnabled = true;
            }
            else
            {
                _moduleConfiguration._accessPointDHCPEnabled = false;
            }
            commandParsed = true;
        }

        private void ParseAccessPointDHCPConfiguration(string response)
        {
            string dhcpData = response.Replace("+CWDHCPS_DEF:", "");
            string[] dhcpParams = dhcpData.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            _moduleConfiguration._accessPointDhcpConfig.leaseTime = dhcpParams[0];
            _moduleConfiguration._accessPointDhcpConfig.startIp = dhcpParams[1];
            _moduleConfiguration._accessPointDhcpConfig.endIp = dhcpParams[2];
            commandParsed = true;
        }

        private void ParseAccessPointIpConfiguration(string response)
        {
            string[] CommandLines = response.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            _moduleConfiguration._accessPointIpConfig.ip = CommandLines[0].Split(':')[2].Replace("\"", "").Trim();
            _moduleConfiguration._accessPointIpConfig.gateway = CommandLines[1].Split(':')[2].Replace("\"", "").Trim();
            _moduleConfiguration._accessPointIpConfig.netmask = CommandLines[2].Split(':')[2].Replace("\"", "").Trim();
            commandParsed = true;
        }
    }
}
