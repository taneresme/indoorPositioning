using System;

namespace IndoorPositioning.UI.Model
{
    public class Beacon
    {
        public int BeaconId { get; set; }
        public string Name { get; set; }
        public string MACAddress { get; set; }
        public DateTime LastSignalTimestamp { get; set; }
        public string BeaconType { get; set; }
        public int LastRssi { get; set; }
    }
}