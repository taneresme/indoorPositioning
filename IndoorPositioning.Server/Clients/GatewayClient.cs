using IndoorPositioning.Server.Database.Dao;
using IndoorPositioning.Server.Database.Model;
using IndoorPositioning.Server.Logging;
using IndoorPositioning.Server.Static;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;

namespace IndoorPositioning.Server.Clients
{
    public class GatewayClient : Client
    {
        private ILogger LOGGER = Logger.CreateLogger<GatewayClient>();

        private static readonly object locker_positioning = new object();

        /* Locker object */
        private static readonly object locker_dataReceived = new object();

        public GatewayClient(TcpClient tcpClient) : base(tcpClient)
        {
            /* Start receiving from socket */
            BeginReceive();
        }

        public override void DataReceived(string data)
        {
            /* Locking the operation just in case. */
            lock (locker_dataReceived)
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
                    
                    /* Beahve according to the mode of the server */
                    if (ServerSettings.ServerMode == Enums.ServerModes.Fingerprinting)
                    {
                        Fingerprint(beaconMac, gatewayMac, rssi);
                    }
                    else if (ServerSettings.ServerMode == Enums.ServerModes.Positioning)
                    {
                        Position(beaconMac, gatewayMac, rssi);
                    }
                    else
                    {
                        // do nothing.
                    }

                    CheckGateway(gatewayMac, gatewayType);
                    CheckBeacon(beaconMac, rssi);
                }
                catch (Exception ex)
                {
                    LOGGER.LogError(ex.ToString());
                    LOGGER.LogError(data);
                }
            }
        }

        private void Position(string beaconMac, string gatewayMac, int rssi)
        {
            /* If the received signal is not from the beacon to be used positioning, ignore it */
            if (!PositioningParams.Positioning_BeaconMacAddress.Equals(beaconMac))
            {
                return;
            }

            /* Get gateway */
            GatewayDao gwDao = new GatewayDao();
            Gateway gateway = gwDao.GetGateway(gatewayMac);

            PositioningParams.SetRssi(gateway.GatewayId, rssi);
        }

        /* Stores the signals with the provided coordinates. */
        private void Fingerprint(string beaconMac, string gatewayMac, int Rssi)
        {
            /* If the received signal is not from the beacon to be used fingerprinting, ignore it */
            if (!FingerprintingSettings.Fingerprinting_BeaconMacAddress.Equals(beaconMac))
            {
                return;
            }

            /* Get gateway */
            GatewayDao gwDao = new GatewayDao();
            Gateway gateway = gwDao.GetGateway(gatewayMac);

            FingerprintingDao dao = new FingerprintingDao();
            Fingerprinting fingerprinting = new Fingerprinting()
            {
                GatewayId = gateway.GatewayId,
                Rssi = Rssi,
                Timestamp = DateTime.Now,
                Xaxis = FingerprintingSettings.Fingerprinting_X,
                Yaxis = FingerprintingSettings.Fingerprinting_Y,
                EnvironmentId = FingerprintingSettings.Fingerprinting_EnvironmentId
            };
            dao.NewFingerprint(fingerprinting);
        }

        /* Check whether the gateway was registred or now
         * If not, it will create a new one otherwise updated last RSSI */
        private void CheckGateway(string gatewayMac, string gatewayType)
        {
            /* Check the GW */
            GatewayDao dao = new GatewayDao();
            Gateway gateway = dao.GetGateway(gatewayMac);
            if (gateway == null)
            {
                /* There is no definition for this GW. 
                 * Create a new gateway record */
                gateway = new Gateway()
                {
                    GatewayType = gatewayType,
                    MACAddress = gatewayMac,
                    Name = "Unknown",
                    Xaxis = "0",
                    Yaxis = "0",
                };

                /* save new gateway */
                dao.NewGateway(gateway);
            }
            else
            {
                /* Update beacon */
                gateway.LastSignalTimestamp = DateTime.Now;
                dao.UpdateGateway(gateway);
            }
        }

        /* Check whether the beacon was registred or now
         * If not, it will create a new one otherwise updated last RSSI
         * and timestamp */
        private void CheckBeacon(string beaconMac, int rssi)
        {
            /* Check the beacon */
            BeaconDao dao = new BeaconDao();
            Beacon beacon = dao.GetBeacon(beaconMac);
            if (beacon == null)
            {
                /* There is no definition for this beacon. 
                * Create a new beacon record */
                beacon = new Beacon()
                {
                    MACAddress = beaconMac,
                    LastRssi = rssi,
                    Name = "Unknown",
                };

                /* save new beacon */
                dao.NewBeacon(beacon);
            }
            else
            {
                /* Update beacon */
                beacon.LastSignalTimestamp = DateTime.Now;
                beacon.LastRssi = rssi;
                dao.UpdateBeacon(beacon);
            }
        }
    }
}
