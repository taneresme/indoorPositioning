using IndoorPositioning.Server.Database.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IndoorPositioning.Server.Database
{
    public class IndoorPositioningContext : DbContext
    {
        public DbSet<Beacon> Beacons { get; set; }
        public DbSet<Gateway> Gateways { get; set; }
        public DbSet<Fingerprinting> Fingerprintings { get; set; }
        public DbSet<Environment> Environments { get; set; }
        public DbSet<EnvironmentReferencePoint> EnvironmentReferencePoints { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(Config.AppSettings.DataSource);
        }
    }
}
