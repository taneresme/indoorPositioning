using System;

namespace IndoorPositioning.Server.Database.Model
{
    public class Beacon
    {
        public int BeaconId { get; set; }
        public string Name { get; set; }
        public string MACAddress { get; set; }
        public DateTime LastSignalTimestamp { get; set; } = DateTime.Now;
        public string BeaconType { get; set; }
        public int LastRssi { get; set; }
    }
}