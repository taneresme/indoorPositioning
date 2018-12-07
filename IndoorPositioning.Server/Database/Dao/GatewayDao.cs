using IndoorPositioning.Server.Database.Model;
using System.Linq;

namespace IndoorPositioning.Server.Database.Dao
{
    public class GatewayDao
    {
        /* Finds the gateway with macAddress */
        public Gateway GetGateway(string macAddress)
        {
            using (IndoorPositioningContext db = new IndoorPositioningContext())
            {
                var gateway = db.Gateways
                    .Where(b => b.MACAddress == macAddress)
                    .FirstOrDefault();

                return gateway;
            }
        }

        /* Adds new gateway */
        public Gateway NewGateway(Gateway gateway)
        {
            using (IndoorPositioningContext db = new IndoorPositioningContext())
            {
                db.Gateways.Add(gateway);
                db.SaveChanges();

                return gateway;
            }
        }
    }
}
