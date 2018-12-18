using IndoorPositioning.Server.Database.Model;

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
    }
}
