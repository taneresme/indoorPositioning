using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IndoorPositioning.Beacon.Core;

namespace IndoorPositioning.Beacon.Bluetooth
{
    public class Beacon : IBeacon
    {
        public bool IsProximityBeacon { get; set; }
        public string Address
        {
            get
            {
                return string.Join("", BitConverter.GetBytes(AddressAsUlong)
                    .Reverse()
                    .Select(b => b.ToString("X2")))
                    .Substring(6);
            }
        }
        public ulong AddressAsUlong { get; set; }
        public string LocalName { get; set; }
        public List<Guid> ServiceUuids { get; set; }
        public List<BeaconDataSection> DataSections { get; set; }
        public List<BeaconManufacturerData> ManufacturerData { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Rssi { get; set; }

        public Beacon()
        {
            ServiceUuids = new List<Guid>();
            DataSections = new List<BeaconDataSection>();
            ManufacturerData = new List<BeaconManufacturerData>();
        }

        public void Update(IBeacon beacon)
        {
            this.AddressAsUlong = beacon.AddressAsUlong;
            this.DataSections = beacon.DataSections;
            this.IsProximityBeacon = beacon.IsProximityBeacon;
            this.LocalName = beacon.LocalName;
            this.ManufacturerData = beacon.ManufacturerData;
            this.ServiceUuids = beacon.ServiceUuids;
            this.UpdatedAt = beacon.UpdatedAt;
            this.Rssi = beacon.Rssi;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder()
                .Append("Beacon [ ")
                .AppendLine("IsProximityBeacon : " + IsProximityBeacon)
                .AppendLine("Address : " + Address)
                .AppendLine("LocalName : " + LocalName)
                .AppendLine("Rssi : " + Rssi);
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