using System;
using System.Text;

namespace IndoorPositioning.Beacon.Core
{
    public class BeaconOperationEventArgs : EventArgs
    {
        public IBeacon Beacon { get; set; }
        public int Index { get; set; }

        public override string ToString()
        {
            return new StringBuilder()
                .Append("BeaconOperationEventArgs [ ")
                .AppendLine("Beacon : " + Beacon.ToString())
                .AppendLine("Index : " + Index.ToString())
                .Append(" ]")
                .ToString();
        }
    }
}
