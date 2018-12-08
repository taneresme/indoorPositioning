using System;

namespace IndoorPositioning.UI.Model
{
    public class Fingerprinting
    {
        public int FingerprintingId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Rssi { get; set; }

        public int EnvironmentId { get; set; }
        public Environment Environment { get; set; }

        public int GatewayId { get; set; }
        public Gateway Gateway { get; set; }

        public int BeaconId { get; set; }
        public Beacon Beacon { get; set; }
    }
}
