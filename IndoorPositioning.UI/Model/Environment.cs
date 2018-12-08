using System;

namespace IndoorPositioning.UI.Model
{
    public class Environment
    {
        public int EnvironmentId { get; set; }
        public string Name { get; set; }
        public DateTime Timestamp { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
