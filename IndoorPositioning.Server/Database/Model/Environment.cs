
using System;

namespace IndoorPositioning.Server.Database.Model
{
    public class Environment
    {
        public int EnvironmentId { get; set; }
        public string Name { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
