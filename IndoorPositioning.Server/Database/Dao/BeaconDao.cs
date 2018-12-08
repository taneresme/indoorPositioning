using IndoorPositioning.Server.Database.Model;
using System.Collections.Generic;
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

        /* Returns all beacons in DB */
        public List<Beacon> GetBeacons()
        {
            using (IndoorPositioningContext db = new IndoorPositioningContext())
            {
                var beacons = db.Beacons.ToList();
                return beacons;
            }
        }

        /* Returns the beacon with given id */
        public Beacon GetBeaconById(int id)
        {
            using (IndoorPositioningContext db = new IndoorPositioningContext())
            {
                var beacon = db.Beacons
                    .Where(b => b.BeaconId == id)
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

        /* Updates the given beacon */
        public Beacon UpdateBeacon(Beacon beacon)
        {
            using (IndoorPositioningContext db = new IndoorPositioningContext())
            {
                db.Beacons.Update(beacon);
                db.SaveChanges();

                return beacon;
            }
        }

        /* Deletes the given beacon */
        public Beacon DeleteBeacon(Beacon beacon)
        {
            using (IndoorPositioningContext db = new IndoorPositioningContext())
            {
                db.Beacons.Remove(beacon);
                db.SaveChanges();

                return beacon;
            }
        }
    }
}
