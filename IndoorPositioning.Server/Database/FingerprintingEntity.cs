using System;

namespace IndoorPositioning.Server.Database
{
    public class FingerprintingEntity
    {
        public int FingerprintingId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Rssi { get; set; }

        public int BeaconId { get; set; }
        public BeaconEntity Beacon { get; set; }

        public int GatewayId { get; set; }
        public GatewayEntity Gateway { get; set; }
    }
}
