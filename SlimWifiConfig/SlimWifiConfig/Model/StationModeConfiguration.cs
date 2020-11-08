namespace SlimWifiConfig.Model
{
    public class StationModeConfiguration
    {
        private string connectedNetworkName;
        private bool dhcpSetting;
        private IpConfiguration ipConfig;   // Retrive and set by AT+CIPSTA

        public string ConnectedNetworkName { get => connectedNetworkName; set => connectedNetworkName = value; }
        public bool DhcpSetting { get => dhcpSetting; set => dhcpSetting = value; }
        public IpConfiguration IpConfig { get => ipConfig; set => ipConfig = value; }
    }
}
