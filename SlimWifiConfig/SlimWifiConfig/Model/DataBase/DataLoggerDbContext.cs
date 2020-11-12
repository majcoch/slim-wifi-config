using System.Data.Entity;

namespace SlimWifiConfig.Model.DataBase
{
    public class DataLoggerDbContext: DbContext
    {
        public DbSet<Record> Records { get; set; }
    }
}
