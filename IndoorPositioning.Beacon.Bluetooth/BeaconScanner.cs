using IndoorPositioning.Beacon.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;

namespace IndoorPositioning.Beacon.Bluetooth
{
    public class BeaconScanner : IBeaconScanner
    {
        private BluetoothLEAdvertisementWatcher watcher;

        public BeaconScannerStatus Status { get { return (BeaconScannerStatus)watcher.Status; } }

        public event EventHandler<BeaconOperationEventArgs> BeaconSignalReceived;

        private ulong localAddress;
        public string LocalAddress
        {
            get
            {
                return string.Join("", BitConverter.GetBytes(localAddress)
                      .Reverse()
                      .Select(b => b.ToString("X2")))
                      .Substring(4); ;
            }
        }

        public BeaconScanner()
        {
            SetLocalAddress().Wait();

            watcher = new BluetoothLEAdvertisementWatcher();
            watcher.Received += _watcher_Received;
        }

        private async Task SetLocalAddress()
        {
            var bluetooth = await Windows.Devices.Bluetooth.BluetoothAdapter.GetDefaultAsync();
            localAddress = bluetooth.BluetoothAddress;
        }

        public void Start()
        {
            watcher.Start();
        }

        public void Stop()
        {
            watcher.Stop();
        }

        private void _watcher_Received(BluetoothLEAdvertisementWatcher sender,
            BluetoothLEAdvertisementReceivedEventArgs args)
        {
            var beacon = new Beacon
            {
                AddressAsUlong = args.BluetoothAddress,
                LocalName = args.Advertisement.LocalName,
                UpdatedAt = args.Timestamp.UtcDateTime,
                Rssi = args.RawSignalStrengthInDBm
            };

            //if (args.Advertisement.ManufacturerData != null)
            //{
            //    if (args.Advertisement.ManufacturerData.Count > 0)
            //    {
            //        beacon.IsProximityBeacon = true;
            //    }

            //    foreach (var manufacturerData in args.Advertisement.ManufacturerData)
            //    {
            //        beacon.ManufacturerData.Add(new BeaconManufacturerData(manufacturerData.CompanyId,
            //            manufacturerData.Data.ToArray()));
            //    }
            //}

            //if (args.Advertisement.ServiceUuids != null)
            //{
            //    beacon.ServiceUuids.AddRange(args.Advertisement.ServiceUuids);
            //}

            //if (args.Advertisement.DataSections != null)
            //{
            //    foreach (var dataSections in args.Advertisement.DataSections)
            //    {
            //        beacon.DataSections.Add(new BeaconDataSection(dataSections.DataType,
            //            dataSections.Data.ToArray()));
            //    }
            //}

            BeaconSignalReceived(this, new BeaconOperationEventArgs
            {
                Beacon = beacon,
                LocalAddress = this.LocalAddress
            });
        }
    }
}