using IndoorPositioning.Server.Database.Model;
using System.Collections.Generic;
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

        /* Returns all gateways in DB */
        public List<Gateway> GetGateways()
        {
            using (IndoorPositioningContext db = new IndoorPositioningContext())
            {
                var gateways = db.Gateways.ToList();
                return gateways;
            }
        }

        /* Returns the gateway with given id */
        public Gateway GetGatewayById(int id)
        {
            using (IndoorPositioningContext db = new IndoorPositioningContext())
            {
                var gateway = db.Gateways
                    .Where(b => b.GatewayId == id)
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

        /* Updates new gateway */
        public Gateway UpdateGateway(Gateway gateway)
        {
            using (IndoorPositioningContext db = new IndoorPositioningContext())
            {
                db.Gateways.Update(gateway);
                db.SaveChanges();

                return gateway;
            }
        }
    }
}
