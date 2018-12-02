using System;

namespace IndoorPositioning.Server.Database
{
    public class BeaconEntity
    {
        public int BeaconId { get; set; }
        public string Name { get; set; }
        public string MACAddress { get; set; }
        public DateTime LastSignalTimestamp { get; set; }
        public string BeaconType { get; set; }
    }
}
