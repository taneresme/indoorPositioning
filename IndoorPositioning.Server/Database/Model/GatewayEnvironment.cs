
namespace IndoorPositioning.Server.Database.Model
{
    public class GatewayEnvironment
    {
        public int GatewayEnvironmentId { get; set; }

        public int EnvironmentId { get; set; }
        public Environment Environment { get; set; }

        public int GatewayId { get; set; }
        public Gateway Gateway { get; set; }
    }
}
