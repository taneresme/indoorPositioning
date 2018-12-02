using System;
using System.Collections.Generic;
using System.Text;
using IndoorPositioning.Beacon.Core;

namespace IndoorPositioning.Beacon.Bluetooth
{
    public class Beacon : IBeacon
    {
        public bool IsProximityBeacon { get; set; }
        public ulong Address { get; set; }
        public string LocalName { get; set; }
        public List<Guid> ServiceUuids { get; set; }
        public List<BeaconDataSection> DataSections { get; set; }
        public List<BeaconManufacturerData> ManufacturerData { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Beacon()
        {
            ServiceUuids = new List<Guid>();
            DataSections = new List<BeaconDataSection>();
            ManufacturerData = new List<BeaconManufacturerData>();
        }

        public void Update(IBeacon beacon)
        {
            this.Address = beacon.Address;
            this.DataSections = beacon.DataSections;
            this.IsProximityBeacon = beacon.IsProximityBeacon;
            this.LocalName = beacon.LocalName;
            this.ManufacturerData = beacon.ManufacturerData;
            this.ServiceUuids = beacon.ServiceUuids;
            this.UpdatedAt = beacon.UpdatedAt;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder()
                .Append("Beacon [ ")
                .AppendLine("IsProximityBeacon : " + IsProximityBeacon)
                .AppendLine("Address : " + Address)
                .AppendLine("LocalName : " + LocalName);
            foreach (var item in ServiceUuids)
            {
                sb.AppendLine(item.ToString());
            }
            foreach (var item in DataSections)
            {
                sb.AppendLine(item.ToString());
            }
            foreach (var item in ManufacturerData)
            {
                sb.AppendLine(item.ToString());
            }
            sb.Append(" ]");
            return sb.ToString();
        }
    }
}