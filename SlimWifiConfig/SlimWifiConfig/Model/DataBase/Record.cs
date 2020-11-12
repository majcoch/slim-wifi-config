using System;

namespace SlimWifiConfig.Model.DataBase
{
    public class Record
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Data { get; set; }
    }
}
