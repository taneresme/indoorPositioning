using IndoorPositioning.Server.Clients;

namespace IndoorPositioning.Server.Services
{
    public class SetService : BaseService, IService
    {
        public SetService(ServiceClient serviceClient) : base(serviceClient) { }

        public void Service(string data)
        {
            string inData = data.ToLower();
            string[] dataItems = inData.Split(' ');

            /* Check data */
            if (dataItems.Length < 2)
            {
                ServiceClient.Send(INVALID_PARAMETERS_ERROR);
                return;
            }

            string command = dataItems[1];

            /* Determine the command */
            if ("mode".Equals(command)) SetMode(dataItems);
            else ServiceClient.Send(UNKNOWN_COMMAND_ERROR);
        }

        /* Updates the beacon that is provided as json */
        private void SetMode(string[] dataItems)
        {
            /* Check data */
            if (dataItems.Length < 3)
            {
                ServiceClient.Send(INVALID_PARAMETERS_ERROR);
                return;
            }

            string mode = dataItems[2];

            /* If mode string is invalid */
            if (!Server.Modes.ContainsKey(mode))
            {
                ServiceClient.Send(INVALID_PARAMETERS_ERROR);
                return;
            }

            /* Set mode */
            Server.ServerMode = Server.Modes[mode];
            ServiceClient.Send(OK);
        }
    }
}
