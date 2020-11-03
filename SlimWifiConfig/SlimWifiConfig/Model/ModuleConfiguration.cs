using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlimWifiConfig.Model
{
    enum ModuleMode
    {
        STATION,
        SOFT_AP,
        STATION_AND_AP
    }

    struct ModuleIpConfig
    {
        string ip;
        string gateway;
        string netmask;
    }

    struct ModuleDhcpConfig
    {
        int leaseTime;
        string startIp;
        string endIp;
    }

    class ModuleConfiguration
    {
        private bool _commanEchoing;
        private int _baudrate;
        private ModuleMode _mode;
        
        private bool _accessPointDHCPEnabled;
        private ModuleDhcpConfig _accessPointDhcpConfig;
        private ModuleIpConfig _accessPointIpConfig;
        private string _hostName;

        private string _connectedNetworkName;
        private string _connectedNetworkPassword;
        private bool _networkAutoConnectEnabled;
        private bool _stationDHCPEnabled;
        private ModuleIpConfig _stationIpConfig;
    }
}
