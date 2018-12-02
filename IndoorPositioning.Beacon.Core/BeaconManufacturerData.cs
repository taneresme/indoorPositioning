
using System.Text;

namespace IndoorPositioning.Beacon.Core
{
    public class BeaconManufacturerData
    {
        public ushort CompanyId { get; set; }
        public byte[] Data { get; set; }

        public BeaconManufacturerData(ushort companyId, byte[] data)
        {
            CompanyId = companyId;
            Data = data;
        }

        public override string ToString()
        {
            return new StringBuilder()
                .Append("BeaconManufacturerData [ ")
                .AppendLine("CompanyId : " + CompanyId)
                .AppendLine("Data : " + Encoding.ASCII.GetString(Data))
                .Append(" ]")
                .ToString();
        }

    }
}
