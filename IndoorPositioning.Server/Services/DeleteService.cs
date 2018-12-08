using IndoorPositioning.Server.Clients;
using IndoorPositioning.Server.Database.Dao;
using IndoorPositioning.Server.Database.Model;
using Newtonsoft.Json;

namespace IndoorPositioning.Server.Services
{
    public class DeleteService : BaseService, IService
    {
        public DeleteService(ServiceClient serviceClient) : base(serviceClient) { }

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
            if ("beacon".Equals(command)) DeleteBeacon(data);
            else if ("gateway".Equals(command)) DeleteGateway(data);
            else ServiceClient.Send(UNKNOWN_COMMAND_ERROR);
        }

        /* Updates the beacon that is provided as json */
        private void DeleteBeacon(string data)
        {
            /* we are triming data below, splitting it by blank chars is not a valid way.
             * because blank char can become in the json object as well */
            string command = "delete beacon ";
            string json = data.Substring(command.Length);

            Beacon beacon = JsonConvert.DeserializeObject<Beacon>(json);
            BeaconDao beaconDao = new BeaconDao();
            beaconDao.DeleteBeacon(beacon);
            ServiceClient.Send(OK);
        }

        /* Updates the gateway that is provided as json */
        private void DeleteGateway(string data)
        {
            /* we are triming data below, splitting it by blank chars is not a valid way.
             * because blank char can become in the json object as well */
            string command = "delete gateway ";
            string json = data.Substring(command.Length);

            Gateway gateway = JsonConvert.DeserializeObject<Gateway>(json);
            GatewayDao gatewayDao = new GatewayDao();
            gatewayDao.DeleteGateway(gateway);
            ServiceClient.Send(OK);
        }
    }
}
