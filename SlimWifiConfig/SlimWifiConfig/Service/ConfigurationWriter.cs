using SlimWifiConfig.Model;

namespace SlimWifiConfig.Service
{
    public class ConfigurationWriter
    {
        private ModuleConfiguration _moduleConfiguration;
        private CommandProcessingService _commandProcessingService;
        private bool commandParsed;

        public ConfigurationWriter(CommandProcessingService commandProcessingService, ModuleConfiguration ModuleConfiguration)
        {
            _commandProcessingService = commandProcessingService;
            _moduleConfiguration = ModuleConfiguration;
            commandParsed = false;
        }

        public void WriteModuleConfiguration()
        {
            _commandProcessingService.OnCommandSuccess += OnMultipleConnectionSettingSuccess;
            if (_moduleConfiguration.MultipleConnections) _commandProcessingService.ExecuteCommand("AT+CIPMUX=1", 100);
            else _commandProcessingService.ExecuteCommand("AT+CIPMUX=0", 100);
            commandParsed = false;
            while (!commandParsed) ;
            _commandProcessingService.OnCommandSuccess -= OnMultipleConnectionSettingSuccess;

            if (_moduleConfiguration.Mode == ModuleMode.STATION)
            {
                SetMode((int)ModuleMode.STATION);
                WriteStationConfiguration();
            }
            else if (_moduleConfiguration.Mode == ModuleMode.SOFT_AP)
            {
                SetMode((int)ModuleMode.SOFT_AP);
                WriteAccessPointConfiguration();
            }
            else
            {
                SetMode((int)ModuleMode.STATION_AND_AP);
                WriteStationConfiguration();
                WriteAccessPointConfiguration();
            }
        }

        private void SetMode(int mode)
        {
            _commandProcessingService.OnCommandSuccess += OnModeSelectionSuccess;
            _commandProcessingService.ExecuteCommand($"AT+CWMODE={mode}", 100);
            commandParsed = false;
            while (!commandParsed) ;
            _commandProcessingService.OnCommandSuccess -= OnModeSelectionSuccess;
        }

        private void OnMultipleConnectionSettingSuccess()
        {
            commandParsed = true;
        }

        private void OnModeSelectionSuccess()
        {
            commandParsed = true;
        }

        private void WriteStationConfiguration()
        {
            _commandProcessingService.OnCommandSuccess += OnSetDHCPSettingSuccess;
            if( _moduleConfiguration.StationConfiguration.DhcpSetting)  _commandProcessingService.ExecuteCommand("AT+CWDHCP_DEF=1,1", 100);
            else _commandProcessingService.ExecuteCommand("AT+CWDHCP_DEF=1,0", 100);
            commandParsed = false;
            while (!commandParsed) ;
            _commandProcessingService.OnCommandSuccess -= OnSetDHCPSettingSuccess;


            string StaticIp = _moduleConfiguration.StationConfiguration.IpConfig.ip;
            string StaticGateway = _moduleConfiguration.StationConfiguration.IpConfig.gateway;
            string StaticNetmask = _moduleConfiguration.StationConfiguration.IpConfig.netmask;
            _commandProcessingService.OnCommandSuccess += OnSetIpConfigurationSuccess;
            _commandProcessingService.ExecuteCommand($"AT+CIPSTA=\"{StaticIp}\",\"{StaticGateway}\",\"{StaticNetmask}\"", 100);
            commandParsed = false;
            while (!commandParsed) ;
            _commandProcessingService.OnCommandSuccess -= OnSetIpConfigurationSuccess;
        }

        private void OnSetDHCPSettingSuccess()
        {
            commandParsed = true;
        }

        private void OnSetIpConfigurationSuccess()
        {
            commandParsed = true;
        }

        private void WriteAccessPointConfiguration()
        {
            _commandProcessingService.OnCommandSuccess += OnSetDHCPSettingSuccess;
            if (!_moduleConfiguration.AccessPointConfiguration.DhcpSetting)
            {
                string LeaseTime = _moduleConfiguration.AccessPointConfiguration.DhcpConfig.leaseTime;
                string StartIp = _moduleConfiguration.AccessPointConfiguration.DhcpConfig.startIp;
                string EndIp = _moduleConfiguration.AccessPointConfiguration.DhcpConfig.endIp;
                _commandProcessingService.ExecuteCommand($"AT+CWDHCPS_DEF=1,{LeaseTime},\"{StartIp}\",\"{EndIp}\"", 100);
            }
            else
            {
                _commandProcessingService.ExecuteCommand("AT+CWDHCPS_DEF=0", 100);
            }
            commandParsed = false;
            while (!commandParsed) ;
            _commandProcessingService.OnCommandSuccess -= OnSetDHCPSettingSuccess;

            _commandProcessingService.OnCommandSuccess += OnSetAccessPointConfigSucces;
            string AccessPointName = _moduleConfiguration.AccessPointConfiguration.Name;
            string AccessPointPassword = _moduleConfiguration.AccessPointConfiguration.Password;
            string AccessPointChannelId = _moduleConfiguration.AccessPointConfiguration.ChannelId;
            int AccessPointAuthentication = (int)_moduleConfiguration.AccessPointConfiguration.AuthMethod;

            _commandProcessingService.ExecuteCommand($"AT+CWSAP=\"{AccessPointName}\",\"{AccessPointPassword}\",{AccessPointChannelId},{AccessPointAuthentication}", 300);
            commandParsed = false;
            while (!commandParsed) ;
            _commandProcessingService.OnCommandSuccess -= OnSetAccessPointConfigSucces;
        }

        private void OnSetAccessPointConfigSucces()
        {
            commandParsed = true;
        }
    }
}
