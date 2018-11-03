using IndoorPositioning.Beacon.Core;

namespace IndoorPositioning.Beacon.Bluetooth
{
    public class BeaconScanner : IBeaconScanner
    {
        private IBeacon _beacon;

        public BeaconScanner()
        {
            _beacon = new Beacon();
        }
    }
}