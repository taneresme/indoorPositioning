using IndoorPositioning.Server.Database.Model;
using System.Linq;

namespace IndoorPositioning.Server.Database.Dao
{
    public class BeaconDao
    {
        /* Finds the beacon with macAddress */
        public Beacon GetBeacon(string macAddress)
        {
            using (IndoorPositioningContext db = new IndoorPositioningContext())
            {
                var beacon = db.Beacons
                    .Where(b => b.MACAddress == macAddress)
                    .FirstOrDefault();

                return beacon;
            }
        }

        /* Adds new beacon */
        public Beacon NewBeacon(Beacon beacon)
        {
            using (IndoorPositioningContext db = new IndoorPositioningContext())
            {
                db.Beacons.Add(beacon);
                db.SaveChanges();

                return beacon;
            }
        }

        /* Adds new beacon */
        public Beacon UpdateBeacon(Beacon beacon)
        {
            using (IndoorPositioningContext db = new IndoorPositioningContext())
            {
                db.Beacons.Update(beacon);
                db.SaveChanges();

                return beacon;
            }
        }
    }
}
