namespace SlimWifiConfig.Model
{
    public class LocalServerConfiguration
    {
        private string _LocalAddress;
        private string _LocalPort;

        public LocalServerConfiguration()
        {
            _LocalAddress = "192.168.0.15";
            _LocalPort = "13000";
        }

        public string LocalAddress { get => _LocalAddress; set => _LocalAddress = value; }
        public string LocalPort { get => _LocalPort; set => _LocalPort = value; }
    }
}
