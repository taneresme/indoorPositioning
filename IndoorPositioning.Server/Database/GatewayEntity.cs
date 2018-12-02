using System;

namespace IndoorPositioning.Server.Database
{
    public class GatewayEntity
    {
        public int GatewayId { get; set; }
        public string Name { get; set; }
        public string MACAddress { get; set; }
        public DateTime LastSignalTimestamp { get; set; }
    }
}
