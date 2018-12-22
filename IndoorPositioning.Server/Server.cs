using IndoorPositioning.Server.Clients;
using IndoorPositioning.Server.Listener;
using System;
using System.Collections.Generic;

namespace IndoorPositioning.Server
{
    public class Server : IDisposable
    {
        private List<GatewayClient> gateways = new List<GatewayClient>();
        private List<ServiceClient> services = new List<ServiceClient>();

        private GatewayListener gatewayListener = new GatewayListener();
        private ServiceListener serviceListener = new ServiceListener();

        public void Start()
        {
            gatewayListener.NewGatewayConnected += GatewayListener_NewGatewayConnected;
            gatewayListener.BeginListening();

            serviceListener.NewServiceConnected += ServiceListener_NewServiceClientConnected;
            serviceListener.BeginListening();
        }

        /* Service Listener, new client connection accepted event handler */
        private void ServiceListener_NewServiceClientConnected(ServiceClient client)
        {
            client.ConnectionClosed += ServiceClientConnectionClosed;

            /* We are locking the adding operation because in the case 
             * of any connection closed, the list will be modified */
            lock (services)
            {
                services.Add(client);
            }
        }

        /* Gateway Listener, new client connection accepted event handler */
        private void GatewayListener_NewGatewayConnected(GatewayClient client)
        {
            client.ConnectionClosed += GatewayClientConnectionClosed;

            /* We are locking the adding operation because in the case 
             * of any connection closed, the list will be modified */
            lock (gateways)
            {
                gateways.Add(client);
            }
        }

        /* Service client connection closed event handler */
        private void ServiceClientConnectionClosed(Client client)
        {
            /* We are locking the removing operation because at any time
             * new connection can be added. */
            lock (services)
            {
                services.Remove((ServiceClient)client);
            }
        }

        /* Gateway client connection closed event handler */
        private void GatewayClientConnectionClosed(Client client)
        {
            /* We are locking the removing operation because at any time
             * new connection can be added. */
            lock (gateways)
            {
                gateways.Remove((GatewayClient)client);
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Server() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
