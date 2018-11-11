using IndoorPositioning.Beacon.Core;
using Windows.Devices.Bluetooth.Advertisement;

namespace IndoorPositioning.Beacon.Bluetooth
{
    public class BeaconScanner : IBeaconScanner
    {
        private IBeacon _beacon;
        private BluetoothLEAdvertisementWatcher _watcher;

        public BeaconScanner()
        {
            _beacon = new Beacon();
            _watcher = new BluetoothLEAdvertisementWatcher();
            _watcher.Received += _watcher_Received;
        }

        private void _watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            throw new System.NotImplementedException();
        }
    }
}