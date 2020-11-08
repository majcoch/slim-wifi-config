namespace SlimWifiConfig.Model
{
    public enum AccessPointAuthenticationMethod
    {
        OPEN = 0,
        WPA_PSK = 1,
        WPA2_PSK = 2,
        WPA_WPA2_PSK = 3
    }

    public struct AccessPointDHCPConfiguration
    {
        public string leaseTime;
        public string startIp;
        public string endIp;
    }

    public class AccessPointModeConfiguration
    {
        public string Name { get => name; set => name = value; }
        public string Password { get => password; set => password = value; }
        public string ChannelId { get => channelId; set => channelId = value; }
        public AccessPointAuthenticationMethod AuthMethod { get => authMethod; set => authMethod = value; }
        public bool DhcpSetting { get => dhcpSetting; set => dhcpSetting = value; }
        public AccessPointDHCPConfiguration DhcpConfig { get => dhcpConfig; set => dhcpConfig = value; }

        private string name;
        private string password;
        private string channelId;
        private AccessPointAuthenticationMethod authMethod;
        private bool dhcpSetting;
        private AccessPointDHCPConfiguration dhcpConfig;
    }
}
