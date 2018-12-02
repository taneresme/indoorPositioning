using Microsoft.EntityFrameworkCore;

namespace IndoorPositioning.Server.Database
{
    public class IndoorPositioningContext : DbContext
    {
        public DbSet<BeaconEntity> Beacons { get; set; }
        public DbSet<GatewayEntity> Gateways { get; set; }
        public DbSet<FingerprintingEntity> Fingerprintings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(Config.AppSettings.DataSource);
        }
    }
}
