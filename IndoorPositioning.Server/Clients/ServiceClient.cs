using System.Net.Sockets;

namespace IndoorPositioning.Server.Clients
{
    public class ServiceClient : Client
    {
        /* Locker object */
        private static readonly object locker = new object();

        public ServiceClient(TcpClient tcpClient) : base(tcpClient)
        {
            /* Start receiving from socket */
            BeginReceive();
        }

        public override void DataReceived(string data)
        {
            /* Locking the operation just in case. */
            lock (locker)
            {
                if (data == null) return;

                data = data.ToLower();

                if (data.StartsWith("echo")) Echo(data);
                else if (data.StartsWith("get")) Get(data);
                else Send("Command not found!");
            }
        }

        private void Echo(string data)
        {
            Send(data);
        }

        private void Get(string data)
        {
            string response = "";
            string[] param = data.Split(' ');
            if (param.Length < 2)
            {
                Send("Invalid parameters!");
                return;
            }
            if (param[1] == "beacons")
            {

            }
        }
    }
}
