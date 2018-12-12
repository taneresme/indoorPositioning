using System;
using System.Collections.Generic;

namespace IndoorPositioning.Server.Database.Model
{
    public class Environment
    {
        public int EnvironmentId { get; set; }
        public string Name { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public int Width { get; set; }
        public int Height { get; set; }
        public int DistanceBetweenReferencePoints { get; set; }
    }
}
