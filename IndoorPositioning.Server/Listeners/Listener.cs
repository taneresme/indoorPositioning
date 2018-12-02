using IndoorPositioning.Server.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace IndoorPositioning.Server.Listener
{
    /* This is the base TCP Listener class to provide TCP connectivity funtionality */
    public abstract class Listener : IDisposable
    {
        private ILogger LOGGER = Logger.CreateLogger<Listener>();

        private TcpListener listener;
        private bool listening = false;
        private Thread listeningThread;
        private int port;

        private const int THREAD_SLEEP = 1000;

        protected abstract void OnNewClientConnected(TcpClient client);

        public Listener(int port)
        {
            this.port = port;
        }

        public void BeginListening()
        {
            listener = new TcpListener(IPAddress.Any, port);

            if (listening) return;
            listeningThread = new Thread(Listen);
            listeningThread.IsBackground = true;
            listeningThread.Start();

            listening = true;
        }

        public void StopListening()
        {
            listening = false;
        }

        private void Listen()
        {
            try
            {
                /* Try to start the listener */
                while (listening)
                {
                    try
                    {
                        listener.Start();
                        break;
                    }
                    catch (Exception ex)
                    {
                        LOGGER.LogError(ex.ToString());
                    }

                    Thread.Sleep(THREAD_SLEEP);
                }

                /* Listening loop */
                while (listening)
                {
                    /* Check whether there is a pending connection request */
                    if (!listener.Pending())
                    {
                        /* Each 100 ms */
                        Thread.Sleep(100);
                        continue;
                    }

                    /* Accept the connection request */
                    TcpClient client = listener.AcceptTcpClient();

                    OnNewClientConnected(client);
                }
            }
            catch (ThreadAbortException) { }
        }

        #region IDisposable Support

        /* Default Disposal implementation */

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    listening = false;
                    try{ listener.Stop(); }
                    catch { }
                    try { listeningThread.Abort(); }
                    catch { }
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
