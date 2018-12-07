using IndoorPositioning.Server.Logging;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;

namespace IndoorPositioning.Server.Clients
{
    public class ServiceClient : Client
    {
        private ILogger LOGGER = Logger.CreateLogger<ServiceClient>();

        /* Locker object */
        private static readonly object locker = new object();

        /* Stores data */
        private string[] splittedData;

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
                try
                {
                    if (data == null) return;
                    data = data.ToLower();

                    if (data.StartsWith("echo")) Echo(data);
                    else if (data.StartsWith("get ")) Get(data);
                    else Send("Command not found!");
                }
                catch (System.Exception ex)
                {
                    LOGGER.LogError(ex.ToString());

                    try { Send("An exception occured, check the server logs!"); }
                    catch { }
                }
            }
        }

        private bool CheckData(string data, int length)
        {
            splittedData = data.Split(' ');
            return splittedData.Length >= length;
        }

        private void Echo(string data)
        {
            Send(data);
        }

        private void Get(string data)
        {
            /* Check the data received if valid */
            if (!CheckData(data, 2))
            {
                Send("Invalid parameters!");
                return;
            }
            /* Returns server mode (fingerprinting or positioning) */
            if (splittedData[1] == "mode")
            {
                Send(Server.ServerMode.ToString());
            }
        }
    }
}
