using System;

namespace IndoorPositioning.UI.Model
{
    public class Fingerprinting
    {
        public int FingerprintingId { get; set; }
        public DateTime Timestamp { get; set; }
        public int Xaxis { get; set; }
        public int Yaxis { get; set; }
        public string Rssi { get; set; }

        public int EnvironmentId { get; set; }
        public int GatewayId { get; set; }
    }
}
