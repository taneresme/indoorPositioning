using IndoorPositioning.Server.Clients;
using System.Net.Sockets;

namespace IndoorPositioning.Server.Listener
{
    public delegate void NewGatewayConnectedEventHander(GatewayClient client);

    /* This listener will be used by the gateways.
     * Gateways will be connecting to the server through this listener */
    public class GatewayListener : Listener
    {
        public GatewayListener() : base(Config.AppSettings.GatewayPort) { }

        /* The event definition when new gateway is connected */
        public event NewGatewayConnectedEventHander NewGatewayConnected;
        protected override void OnNewClientConnected(TcpClient client)
        {
            NewGatewayConnected?.Invoke(new GatewayClient(client));
        }
    }
}
