﻿using System;

namespace IndoorPositioning.UI.Model
{
    public class Gateway
    {
        public int GatewayId { get; set; }
        public string Name { get; set; }
        public string MACAddress { get; set; }
        public DateTime LastSignalTimestamp { get; set; }
        public string GatewayType { get; set; }
        public string Xaxis { get; set; }
        public string Yaxis { get; set; }
    }
}
