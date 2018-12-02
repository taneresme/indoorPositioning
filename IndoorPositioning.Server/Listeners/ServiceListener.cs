using IndoorPositioning.Server.Clients;
using System.Net.Sockets;

namespace IndoorPositioning.Server.Listener
{
    public delegate void NewServiceConnectedEventHander(ServiceClient client);

    /* This listener wil be used by the applications to get service 
     * from the server such as UI application. */
    public class ServiceListener : Listener
    {
        public ServiceListener() : base(Config.AppSettings.ServicePort) { }

        /* The event definition when new gateway is connected */
        public event NewServiceConnectedEventHander NewServiceConnected;
        protected override void OnNewClientConnected(TcpClient client)
        {
            NewServiceConnected?.Invoke(new ServiceClient(client));
        }
    }
}
