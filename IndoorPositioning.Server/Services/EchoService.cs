using IndoorPositioning.Server.Clients;

namespace IndoorPositioning.Server.Services
{
    public class EchoService : BaseService, IService
    {
        public EchoService(ServiceClient serviceClient) : base(serviceClient) { }

        public void Service(string data)
        {
            ServiceClient.Send(data);
        }
    }
}
