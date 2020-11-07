namespace SlimWifiConfig.Model
{
    public enum ModuleMode
    {
        STATION,
        SOFT_AP,
        STATION_AND_AP
    }

    public struct ModuleIpConfig
    {
        public string ip;
        public string gateway;
        public string netmask;
    }

    public struct ModuleDhcpConfig
    {
        public string leaseTime;
        public string startIp;
        public string endIp;
    }

    public class ModuleConfiguration
    {
        public ModuleMode _mode;

        public string _hostName;
        public string _hostPassword;
        public bool _accessPointDHCPEnabled;
        public ModuleDhcpConfig _accessPointDhcpConfig;
        public ModuleIpConfig _accessPointIpConfig;       

        public string _connectedNetworkName;
        public bool _stationDHCPEnabled;
        public ModuleIpConfig _stationIpConfig;

        public ModuleConfiguration()
        {
            _hostName = "";
            _hostPassword = "";
            _accessPointDHCPEnabled = true;
            _accessPointDhcpConfig = new ModuleDhcpConfig();
            _accessPointIpConfig = new ModuleIpConfig();
            _connectedNetworkName = "";
            _stationDHCPEnabled = true;
            _stationIpConfig = new ModuleIpConfig();
        }
    }
}
