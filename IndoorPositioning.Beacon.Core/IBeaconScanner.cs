using System;

namespace IndoorPositioning.Beacon.Core
{
    public interface IBeaconScanner
    {
        event EventHandler<BeaconOperationEventArgs> BeaconSignalReceived;

        BeaconScannerStatus Status { get; }
        string LocalAddress { get; }

        void Start();
        void Stop();
    }
}