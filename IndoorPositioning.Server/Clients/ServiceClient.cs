using IndoorPositioning.Server.Logging;
using IndoorPositioning.Server.Services;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;

namespace IndoorPositioning.Server.Clients
{
    public class ServiceClient : Client
    {
        private ILogger LOGGER = Logger.CreateLogger<ServiceClient>();

        private const string UNKNOWN_COMMAND_ERROR = "error: unknown command!";
        private const string EXCEPTION_OCCURED_ERROR = "error: an exception occured, check the server logs!";

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
                    LOGGER.LogError("Received: " + data);

                    if (data == null) return;
                    string inData = data.ToLower();

                    if (inData.StartsWith("echo")) Echo(data);
                    else if (inData.StartsWith("get ")) Get(data);
                    else if (inData.StartsWith("update ")) Update(data);
                    else Send(UNKNOWN_COMMAND_ERROR);
                }
                catch (System.Exception ex)
                {
                    LOGGER.LogError(ex.ToString());

                    try { Send(EXCEPTION_OCCURED_ERROR); }
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
            new EchoService(this).Service(data);
        }

        private void Get(string data)
        {
            new GetService(this).Service(data);
        }

        private void Update(string data)
        {
            new UpdateService(this).Service(data);
        }
    }
}
