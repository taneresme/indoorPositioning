
namespace IndoorPositioning.Server.Database.Model
{
    public class EnvironmentReferencePoint
    {
        public int EnvironmentReferencePointId { get; set; }
        public int Xaxis { get; set; }
        public int Yaxis { get; set; }

        public int EnvironmentId { get; set; }
        public Environment Environment { get; set; }
    }
}
