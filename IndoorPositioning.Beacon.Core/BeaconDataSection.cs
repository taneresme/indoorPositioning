
using System.Text;

namespace IndoorPositioning.Beacon.Core
{
    public class BeaconDataSection
    {
        public byte DataType { get; set; }
        public byte[] Data { get; set; }

        public BeaconDataSection(byte dataType, byte[] data)
        {
            DataType = dataType;
            Data = data;
        }

        public override string ToString()
        {
            return new StringBuilder()
                .Append("BeaconDataSection [ ")
                .AppendLine("DataType : " + DataType.ToString())
                .AppendLine("Data : " + Encoding.ASCII.GetString(Data))
                .Append(" ]")
                .ToString();
        }
    }
}
