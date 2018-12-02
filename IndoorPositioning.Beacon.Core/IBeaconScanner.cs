using System;
using System.Collections.Generic;

namespace IndoorPositioning.Beacon.Core
{
    public interface IBeaconScanner
    {
        event EventHandler<BeaconOperationEventArgs> NewBeaconAdded;
        event EventHandler<BeaconOperationEventArgs> BeaconUpdated;

        List<IBeacon> Beacons { get; }
        BeaconScannerStatus Status { get; }
        void Start();
        void Stop();
    }
}