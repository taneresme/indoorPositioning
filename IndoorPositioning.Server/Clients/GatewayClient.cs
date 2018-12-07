using IndoorPositioning.Server.Database.Dao;
using IndoorPositioning.Server.Database.Model;
using IndoorPositioning.Server.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;

namespace IndoorPositioning.Server.Clients
{
    public class GatewayClient : Client
    {
        private ILogger LOGGER = Logger.CreateLogger<GatewayClient>();

        /* Locker object */
        private static readonly object locker = new object();

        public GatewayClient(TcpClient tcpClient) : base(tcpClient)
        {
            /* Start receiving from socket */
            BeginReceive();
        }

        public override void DataReceived(string data)
        {
            /* Locking the operation just in case. */
            lock (locker)
            {
                try
                {
                    if (data == null) return;

                    /* Sample data will be coming from GW.
                     * $GPRP,AC233F23BB8C,F90AF395ECD1,-44,0201061AFF4C000215FDA50693A4E24FB1AFCFC6EB0764782527114CB9C5 */

                    string[] dataItems = data.Split(',');
                    string gatewayType = dataItems[0];
                    string beaconMac = dataItems[1];
                    string gatewayMac = dataItems[2];
                    int rssi = int.Parse(dataItems[3]);
                    string additionalData = dataItems[4];

                    CheckGateway(gatewayMac, gatewayType);
                    CheckBeacon(beaconMac, rssi);
                }
                catch (Exception ex)
                {
                    LOGGER.LogError(ex.ToString());
                }
            }
        }

        private void CheckGateway(string gatewayMac, string gatewayType)
        {
            /* Check the GW */
            GatewayDao gatewayDao = new GatewayDao();
            Gateway gateway = gatewayDao.GetGateway(gatewayMac);
            if (gateway == null)
            {
                /* There is no definition for this GW. 
                 * Create a new gateway record */
                gateway = new Gateway()
                {
                    GatewayType = gatewayType,
                    MACAddress = gatewayMac,
                };

                /* save new gateway */
                gatewayDao.NewGateway(gateway);
            }
        }

        private void CheckBeacon(string beaconMac, int rssi)
        {
            /* Check the beacon */
            BeaconDao beaconDao = new BeaconDao();
            Beacon beacon = beaconDao.GetBeacon(beaconMac);
            if (beacon == null)
            {
                /* There is no definition for this beacon. 
                * Create a new beacon record */
                beacon = new Beacon()
                {
                    MACAddress = beaconMac,
                    LastRssi = rssi,
                };

                /* save new beacon */
                beaconDao.NewBeacon(beacon);
            }
            else
            {
                /* Update beacon */
                beacon.LastSignalTimestamp = DateTime.Now;
                beacon.LastRssi = rssi;
                beaconDao.UpdateBeacon(beacon);
            }
        }
    }
}
