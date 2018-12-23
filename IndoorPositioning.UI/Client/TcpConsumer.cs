using System;
using System.Configuration;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace IndoorPositioning.UI.Client
{
    public delegate void ConnectionClosedEventHandler(TcpConsumer client);
    public delegate void DataReceivedEventHandler(String data);

    public class TcpConsumer : IDisposable
    {
        private static TcpConsumer instance;
        public static TcpConsumer Instance
        {
            get
            {
                if (instance == null) instance = new TcpConsumer();
                return instance;
            }
        }

        private const int READ_BUFFER_SIZE = 8192;
        //private Thread thread;

        /* TcpClient for the client */
        private TcpClient tcpClient;

        /* Connection details of the client */
        public string IpAddress { get; private set; }
        public int Port { get; private set; }
        public bool IsReceiving { get; private set; } = false;

        /* Gives the connection status of the client */
        public bool IsConnected
        {
            get
            {
                return tcpClient == null || tcpClient.Client == null
                    ? false
                    : tcpClient.Connected;
            }
        }

        /* ConnectionClosed event definition */
        public event ConnectionClosedEventHandler ConnectionClosed;
        protected void OnConnectioClosed()
        {
            ConnectionClosed?.Invoke(this);
        }

        public event DataReceivedEventHandler DataReceived;
        protected void OnDataReceived(string data)
        {
            DataReceived?.Invoke(data);
        }

        private TcpConsumer()
        {
            string remoteAddress = ConfigurationSettings.AppSettings["RemoteServer"].ToString();
            IpAddress = remoteAddress.Split(':')[0];
            Port = int.Parse(remoteAddress.Split(':')[1]);

            Connect();
        }

        private void Connect()
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(IpAddress, Port);
        }

        private bool isConnecting = false;
        private void BeginConnect()
        {
            isConnecting = true;
            while (isConnecting)
            {
                try
                {
                    Close();
                    Connect();
                    break;
                }
                catch { }
            }
            isConnecting = false;
        }

        /* Sends the given data */
        public void Send(string data)
        {
            try
            {
                if (tcpClient == null)
                    throw new Exception("tcpClient cannot be null!");

                byte[] bytes = Encoding.ASCII.GetBytes(data);
                tcpClient.Client.Send(bytes, bytes.Length, SocketFlags.None);
            }
            catch (SocketException ex)
            {
                if (!isConnecting)
                {
                    Thread thread = new Thread(BeginConnect)
                    {
                        IsBackground = true,
                    };
                    thread.Start();
                }
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /* Receives, collects and returns the data */
        public string Receive(int timeoutInSeconds)
        {
            long timeout = timeoutInSeconds * TimeSpan.TicksPerMillisecond;

            StringBuilder sb = new StringBuilder();
            byte[] data = ReceiveInternal(timeout);
            if (data != null)
            {
                do
                {
                    sb.Append(Encoding.ASCII.GetString(data));
                    data = ReceiveInternal(100);
                } while (data != null);
            }

            return sb.ToString();
        }

        /* Tries to receive data from the socket in given duration.
         * If the timeout occured then returns null! */
        private byte[] ReceiveInternal(long timeoutInMilliseconds)
        {
            byte[] bytes;

            /* Calculation of the loop count according to the given timeout duration */
            long loopCount = (timeoutInMilliseconds) / 1000;

            while (true)
            {
                /* Poll the socket to read. */
                if (tcpClient.Client.Poll(100000, SelectMode.SelectRead)) break;
                if (tcpClient.Client.Poll(1, SelectMode.SelectError))
                    throw new Exception("Polling Error occured!");

                loopCount--;
                if (loopCount <= 0) return null;
            }

            bytes = new byte[READ_BUFFER_SIZE];
            int read = tcpClient.Client.Receive(bytes, bytes.Length, SocketFlags.None);

            if (read > 0)
            {
                byte[] bytesRead = new byte[read];
                Array.Copy(bytes, bytesRead, bytesRead.Length);
                return bytesRead;
            }

            return null;
        }

        /* Closes the Client's connection */
        private bool isClosed = false; // To detect redundant calls
        public void Close()
        {
            if (isClosed) return;

            try { tcpClient.Client.Shutdown(SocketShutdown.Both); }
            catch { }
            try { tcpClient.Client.Disconnect(false); }
            catch { }
            try { tcpClient.Client.Close(); }
            catch { }
            try { tcpClient.Dispose(); }
            catch { }
            //try { thread.Abort(); }
            //catch { }

            isClosed = true;
            OnConnectioClosed();
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Close();

                    instance = null;
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}
