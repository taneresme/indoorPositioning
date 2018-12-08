using IndoorPositioning.Server.Clients;

namespace IndoorPositioning.Server.Services
{
    public abstract class BaseService
    {
        protected const string INVALID_PARAMETERS_ERROR = "error: invalid parameters!";
        protected const string UNKNOWN_COMMAND_ERROR = "error: unknown command!";
        protected const string NOT_FOUND_ERROR = "error: not found!";
        protected const string OK = "ok";

        public ServiceClient ServiceClient { get; set; }

        public BaseService(ServiceClient serviceClient)
        {
            ServiceClient = serviceClient;
        }
    }
}
