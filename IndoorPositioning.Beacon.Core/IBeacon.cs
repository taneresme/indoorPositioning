using System;
using System.Collections.Generic;

namespace IndoorPositioning.Beacon.Core
{
    public interface IBeacon
    {
        bool IsProximityBeacon { get; set; }
        ulong Address { get; set; }
        string LocalName { get; set; }
        List<Guid> ServiceUuids { get; set; }
        List<BeaconDataSection> DataSections { get; set; }
        List<BeaconManufacturerData> ManufacturerData { get; set; }
        DateTime UpdatedAt { get; set; }

        void Update(IBeacon beacon);
    }
}