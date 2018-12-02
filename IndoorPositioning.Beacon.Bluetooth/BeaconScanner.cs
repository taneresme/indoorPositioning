using IndoorPositioning.Beacon.Core;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth.Advertisement;

namespace IndoorPositioning.Beacon.Bluetooth
{
    public class BeaconScanner : IBeaconScanner
    {
        private BluetoothLEAdvertisementWatcher _watcher;

        public BeaconScannerStatus Status { get { return (BeaconScannerStatus)_watcher.Status; } }
        
        public event EventHandler<BeaconOperationEventArgs> NewBeaconAdded;
        public event EventHandler<BeaconOperationEventArgs> BeaconUpdated;

        private List<IBeacon> _beacons = new List<IBeacon>();
        public List<IBeacon> Beacons { get { return _beacons; } }

        public BeaconScanner()
        {
            _watcher = new BluetoothLEAdvertisementWatcher();
            _watcher.Received += _watcher_Received;
        }

        public void Start()
        {
            _watcher.Start();
        }

        public void Stop()
        {
            _watcher.Stop();
        }

        private void _watcher_Received(BluetoothLEAdvertisementWatcher sender,
            BluetoothLEAdvertisementReceivedEventArgs args)
        {
            var beacon = new Beacon
            {
                Address = args.BluetoothAddress,
                LocalName = args.Advertisement.LocalName,
                UpdatedAt = args.Timestamp.UtcDateTime
            };
            if (args.Advertisement.ManufacturerData != null)
            {
                if (args.Advertisement.ManufacturerData.Count > 0)
                {
                    beacon.IsProximityBeacon = true;
                }

                foreach (var manufacturerData in args.Advertisement.ManufacturerData)
                {
                    beacon.ManufacturerData.Add(new BeaconManufacturerData(manufacturerData.CompanyId,
                        manufacturerData.Data.ToArray()));
                }
            }

            if (args.Advertisement.ServiceUuids != null)
            {
                beacon.ServiceUuids.AddRange(args.Advertisement.ServiceUuids);
            }

            if (args.Advertisement.DataSections != null)
            {
                foreach (var dataSections in args.Advertisement.DataSections)
                {
                    beacon.DataSections.Add(new BeaconDataSection(dataSections.DataType,
                        dataSections.Data.ToArray()));
                }
            }

            for (int i = 0; i < _beacons.Count; i++)
            {
                if (beacon.Address == _beacons[i].Address)
                {
                    _beacons[i].Update(beacon);
                    BeaconUpdated(this, new BeaconOperationEventArgs
                    {
                        Beacon = beacon,
                        Index = i
                    });
                    return;
                }
            }

            _beacons.Add(beacon);
            NewBeaconAdded(this, new BeaconOperationEventArgs
            {
                Beacon = beacon,
                Index = _beacons.Count - 1
            });
        }
    }
}