using IndoorPositioning.Server.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace IndoorPositioning.Server.Clients
{
    public delegate void ConnectionClosedEventHandler(Client client);

    public abstract class Client : IDisposable
    {
        private ILogger LOGGER = Logger.CreateLogger<Client>();

        private Thread thread;

        /* ConnectionClosed event definition */
        public event ConnectionClosedEventHandler ConnectionClosed;
        protected void OnConnectioClosed()
        {
            ConnectionClosed?.Invoke(this);
        }

        /* TcpClient for the client */
        private TcpClient tcpClient;
        public TcpClient TcpClient
        {
            get { return tcpClient; }
            set
            {
                tcpClient = value;
                tcpClient.Client.ReceiveTimeout = 2000;
                tcpClient.Client.SendTimeout = 2000;
            }
        }

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

        /* Initial point for the client */
        public Client(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;

            try
            {
                string remoteAddress = this.tcpClient.Client.RemoteEndPoint.ToString();
                IpAddress = remoteAddress.Split(':')[0];
                Port = int.Parse(remoteAddress.Split(':')[1]);
            }
            catch { /* Do not care the errors */ }
        }

        /* Abstract class definition to be called when data received from the socket */
        public abstract void DataReceived(String data);

        /* Sends the given data */
        public void Send(String data)
        {
            if (TcpClient == null)
                throw new Exception("tcpClient cannot be null!");

            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(data);
                TcpClient.Client.Send(bytes, bytes.Length, SocketFlags.None);
            }
            catch (Exception ex)
            {
                LOGGER.LogError(ex.ToString());
                throw ex;
            }
        }

        /* Tries to receive data from the socket in given duration.
         * If the timeout occured then returns null! */
        public byte[] Receive(int timeoutInSeconds)
        {
            byte[] bytes;

            /* Calculation of the loop count according to the given timeout duration */
            long loopCount = (timeoutInSeconds * TimeSpan.TicksPerMillisecond) / 1000;

            while (true)
            {
                /* Poll the socket to read. */
                if (TcpClient.Client.Poll(1000, SelectMode.SelectRead)) break;
                if (TcpClient.Client.Poll(1, SelectMode.SelectError))
                    throw new Exception("Polling Error occured!");

                loopCount--;
                if (loopCount <= 0) return null;
            }

            bytes = new byte[8192];
            int read = TcpClient.Client.Receive(bytes, bytes.Length, SocketFlags.None);

            if (read > 0)
            {
                byte[] bytesRead = new byte[read];
                Array.Copy(bytes, bytesRead, bytesRead.Length);
                return bytesRead;
            }

            return null;
        }

        /* Initiates a receiving thread */
        public void BeginReceive()
        {
            thread = new Thread(Receive)
            {
                IsBackground = true
            };
            thread.Start();
        }

        private void Receive()
        {
            try
            {
                /* ıf it is already in receiving mode then return */
                if (IsReceiving) return;

                IsReceiving = true;
                while (IsReceiving)
                {
                    StringBuilder sb = new StringBuilder();
                    byte[] data = Receive(1);
                    if (data != null)
                    {
                        do
                        {
                            sb.Append(Encoding.ASCII.GetString(data));

                            data = Receive(1);
                        } while (data != null);

                        DataReceived(sb.ToString());
                    }
                }
            }
            catch (ThreadAbortException) { }
            catch (SocketException ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("ErrorCode: " + ex.ErrorCode)
                    .AppendLine("NativeErrorCode: " + ex.NativeErrorCode)
                    .AppendLine("SocketErrorCode: " + ex.SocketErrorCode)
                    .AppendLine("Source: " + ex.Source)
                    .AppendLine(ex.ToString());
                LOGGER.LogError(sb.ToString());
            }
            catch (Exception ex)
            {
                LOGGER.LogError(ex.ToString());
            }
        }

        /* Closes the Client's connection */
        private bool isClosed = false; // To detect redundant calls
        public void Close()
        {
            if (isClosed) return;

            try { TcpClient.Client.Shutdown(SocketShutdown.Both); }
            catch { }
            try { TcpClient.Client.Disconnect(false); }
            catch { }
            try { TcpClient.Client.Close(); }
            catch { }
            try { TcpClient.Dispose(); }
            catch { }
            try { thread.Abort(); }
            catch { }

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
