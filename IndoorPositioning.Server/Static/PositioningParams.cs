using System.Collections.Generic;

namespace IndoorPositioning.Server.Static
{
    public class PositioningParams
    {
        public static int Positioning_BeaconId { get; set; }
        public static string Positioning_BeaconMacAddress { get; set; }
        /* Stores the rssi values coming from the respective gateway. 
         * We are matching gateway id and rssi below. */
        public static Dictionary<int, int> Positioning_BeaconRssi { get; set; } = new Dictionary<int, int>();

        /* Set the RSSI value coming from the given gateway */
        public static void SetRssi(int gatewayId, int rssi)
        {
            if (!Positioning_BeaconRssi.ContainsKey(gatewayId))
            {
                Positioning_BeaconRssi.Add(gatewayId, rssi);
            }
            else
            {
                Positioning_BeaconRssi[gatewayId] = rssi;
            }
        }
    }
}
