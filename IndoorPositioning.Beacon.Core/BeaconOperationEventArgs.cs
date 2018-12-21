using System;
using System.Text;

namespace IndoorPositioning.Beacon.Core
{
    public class BeaconOperationEventArgs : EventArgs
    {
        public IBeacon Beacon { get; set; }
        public string LocalAddress { get; set; }

        public override string ToString()
        {
            return new StringBuilder()
                .Append("BeaconOperationEventArgs [ ")
                .AppendLine("Beacon : " + Beacon.ToString())
                .AppendLine("LocalAddress : " + LocalAddress)
                .Append(" ]")
                .ToString();
        }
    }
}
