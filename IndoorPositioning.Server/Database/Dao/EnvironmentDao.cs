using IndoorPositioning.Server.Database.Model;
using System.Collections.Generic;
using System.Linq;

namespace IndoorPositioning.Server.Database.Dao
{
    public class EnvironmentDao
    {
        /* Returns all environments in DB */
        public List<Environment> GetEnvironments()
        {
            using (IndoorPositioningContext db = new IndoorPositioningContext())
            {
                var beacons = db.Environments.ToList();
                return beacons;
            }
        }

        /* Adds new environment */
        public Environment NewEnvironment(Environment environment)
        {
            using (IndoorPositioningContext db = new IndoorPositioningContext())
            {
                db.Environments.Add(environment);
                db.SaveChanges();

                return environment;
            }
        }

        /* Deletes the given environment */
        public Environment DeleteEnvironment(Environment environment)
        {
            using (IndoorPositioningContext db = new IndoorPositioningContext())
            {
                db.Environments.Remove(environment);
                db.SaveChanges();

                return environment;
            }
        }

        /* Updates the given environment */
        public Environment UpdateEnvironment(Environment environment)
        {
            using (IndoorPositioningContext db = new IndoorPositioningContext())
            {
                db.Environments.Update(environment);
                db.SaveChanges();

                return environment;
            }
        }
    }
}
