using System.Collections.Generic;

namespace IndoorPositioning.UI.Model
{
    public class AdjustedFingerprinting
    {
        public Coordinate Coordinates { get; set; } = new Coordinate();
        /* Contains the list of RSSI values with the gateways
         * If there are 3 gateway, this list will contain 3 items */
        public List<RssiValue> RssiValueAndGateway { get; set; } = new List<RssiValue>();
    }

    public class RssiValue
    {
        public int GatewayId { get; set; }
        public double Rssi { get; set; }
    }

    public class Coordinate
    {
        public int Xaxis { get; set; }
        public int Yaxis { get; set; }
        public int HitCount { get; set; }
    }
}
