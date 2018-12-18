using System;

namespace IndoorPositioning.Server.Database.Model
{
    public class Fingerprinting
    {
        public int FingerprintingId { get; set; }
        public DateTime Timestamp { get; set; }
        public int Xaxis { get; set; }
        public int Yaxis { get; set; }
        public int Rssi { get; set; }

        public int EnvironmentId { get; set; }
        public Environment Environment { get; set; }

        public int GatewayId { get; set; }
        public Gateway Gateway { get; set; }
    }
}
