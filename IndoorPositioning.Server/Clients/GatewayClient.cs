using System.Net.Sockets;

namespace IndoorPositioning.Server.Clients
{
    public class GatewayClient : Client
    {
        /* Locker object */
        private static readonly object locker = new object();

        public GatewayClient(TcpClient tcpClient) : base(tcpClient)
        {
            /* Start receiving from socket */
            BeginReceive();
        }

        public override void DataReceived(string data)
        {
            /* Locking the operation just in case. */
            lock (locker)
            {

            }
        }
    }
}
