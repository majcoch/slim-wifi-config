namespace SlimWifiConfig.Model
{
    public enum ModuleMode
    {
        STATION = 1,
        SOFT_AP = 2,
        STATION_AND_AP = 3
    }

    public struct IpConfiguration
    {
        public string ip;
        public string gateway;
        public string netmask;
    }

    public class ModuleConfiguration
    {
        private bool configurationValid;
        private ModuleMode mode;
        private bool multipleConnections;
        private StationModeConfiguration stationConfiguration;
        private AccessPointModeConfiguration accessPointConfiguration;

        public bool ConfigurationValid { get => configurationValid; set => configurationValid = value; }
        public ModuleMode Mode { get => mode; set => mode = value; }
        public StationModeConfiguration StationConfiguration { get => stationConfiguration; set => stationConfiguration = value; }
        public AccessPointModeConfiguration AccessPointConfiguration { get => accessPointConfiguration; set => accessPointConfiguration = value; }
        public bool MultipleConnections { get => multipleConnections; set => multipleConnections = value; }
    }
}
