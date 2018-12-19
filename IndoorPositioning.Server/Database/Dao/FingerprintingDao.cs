using IndoorPositioning.Server.Database.Model;
using System.Collections.Generic;
using System.Linq;

namespace IndoorPositioning.Server.Database.Dao
{
    public class FingerprintingDao
    {
        /* Adds new fingerprinting record */
        public Fingerprinting NewFingerprint(Fingerprinting fingerprinting)
        {
            using (IndoorPositioningContext db = new IndoorPositioningContext())
            {
                db.Fingerprintings.Add(fingerprinting);
                db.SaveChanges();

                return fingerprinting;
            }
        }

        /* Gets the fingerprinting records of the given environment */
        public List<Fingerprinting> GetFingerprinting(int environmentId)
        {
            using (IndoorPositioningContext db = new IndoorPositioningContext())
            {
                var fingerprintings = db.Fingerprintings
                    .Where(b => b.EnvironmentId == environmentId)
                    .ToList();

                return fingerprintings;
            }
        }
    }
}
