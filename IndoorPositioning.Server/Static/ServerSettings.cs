using IndoorPositioning.Server.Enums;
using System.Collections.Generic;

namespace IndoorPositioning.Server.Static
{
    public class ServerSettings
    {
        public static ServerModes ServerMode { get; set; } = ServerModes.Idle;

        /* String counterparts of server modes */
        public static Dictionary<string, ServerModes> Modes = new Dictionary<string, ServerModes>()
        {
            { "positioning", ServerModes.Positioning},
            { "fingerprinting", ServerModes.Fingerprinting},
            { "idle", ServerModes.Idle},
        };
    }
}
