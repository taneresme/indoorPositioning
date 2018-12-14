using System;
using System.Collections.Generic;

namespace IndoorPositioning.Beacon.Core
{
    public interface IBeacon
    {
        bool IsProximityBeacon { get; set; }
        string Address { get; }
        ulong AddressAsUlong { get; set; }
        string LocalName { get; set; }
        List<Guid> ServiceUuids { get; set; }
        List<BeaconDataSection> DataSections { get; set; }
        List<BeaconManufacturerData> ManufacturerData { get; set; }
        DateTime UpdatedAt { get; set; }
        int Rssi { get; set; }

        void Update(IBeacon beacon);
    }
}