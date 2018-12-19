using IndoorPositioning.Server.Clients;
using IndoorPositioning.Server.Database.Dao;
using IndoorPositioning.Server.Database.Model;
using System.Collections.Generic;
using System.Text;

namespace IndoorPositioning.Server.Services
{
    public class GetService : BaseService, IService
    {
        public GetService(ServiceClient serviceClient) : base(serviceClient) { }

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
            if ("beacons".Equals(command)) GetBeacons();
            else if ("beacon".Equals(command)) GetBeacon(dataItems);
            else if ("gateways".Equals(command)) GetGateways();
            else if ("gateway".Equals(command)) GetGateway(dataItems);
            else if ("mode".Equals(command)) GetMode();
            else if ("environments".Equals(command)) GetEnvironments();
            else if ("fingerprinting".Equals(command)) GetFingerprinting(dataItems);
            else ServiceClient.Send(UNKNOWN_COMMAND_ERROR);
        }

        private void GetBeacons()
        {
            BeaconDao beaconDao = new BeaconDao();
            List<Beacon> beacons = beaconDao.GetBeacons();

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(beacons);
            ServiceClient.Send(json);
        }

        private void GetBeacon(string[] dataItems)
        {
            /* Check data */
            if (dataItems.Length < 4)
            {
                ServiceClient.Send(INVALID_PARAMETERS_ERROR);
                return;
            }

            string where = dataItems[2];
            string whereValue = dataItems[3];

            /* Determine the ID parameter */
            if ("-id".Equals(where)) GetBeaconById(int.Parse(whereValue));
            else ServiceClient.Send(UNKNOWN_COMMAND_ERROR);
        }

        private void GetBeaconById(int id)
        {
            BeaconDao beaconDao = new BeaconDao();
            Beacon beacon = beaconDao.GetBeaconById(id);

            if (beacon == null) {
                ServiceClient.Send(NOT_FOUND_ERROR);
                return;
            }

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(beacon);
            ServiceClient.Send(json);
        }

        private void GetGateways()
        {
            GatewayDao gatewayDao = new GatewayDao();
            List<Gateway> gateways = gatewayDao.GetGateways();

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(gateways);
            ServiceClient.Send(json);
        }

        private void GetGateway(string[] dataItems)
        {
            /* Check data */
            if (dataItems.Length < 4)
            {
                ServiceClient.Send(INVALID_PARAMETERS_ERROR);
                return;
            }

            string where = dataItems[2];
            string whereValue = dataItems[3];

            /* Determine the ID parameter */
            if ("-id".Equals(where)) GetGatewayById(int.Parse(whereValue));
            else ServiceClient.Send(UNKNOWN_COMMAND_ERROR);
        }

        private void GetGatewayById(int id)
        {
            GatewayDao gatewayDao = new GatewayDao();
            Gateway gateway = gatewayDao.GetGatewayById(id);

            if (gateway == null)
            {
                ServiceClient.Send(NOT_FOUND_ERROR);
                return;
            }

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(gateway);
            ServiceClient.Send(json);
        }

        private void GetMode()
        {
            ServiceClient.Send(Server.ServerMode.ToString());
        }

        private void GetEnvironments()
        {
            EnvironmentDao environmentDao = new EnvironmentDao();
            List<Environment> environments = environmentDao.GetEnvironments();

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(environments);
            ServiceClient.Send(json);
        }

        private void GetFingerprinting(string[] dataItems)
        {
            /* Create error message */
            StringBuilder sb = new StringBuilder()
                .AppendLine(INVALID_PARAMETERS_ERROR)
                .AppendLine("Sample: get fingerprinting -env 2")
                .AppendLine("-env: environment id ");

            /* The parameters are valid? */
            if (dataItems.Length < 4)
            {
                ServiceClient.Send(sb.ToString());
                return;
            }

            /* Check the parameters */
            if (!"-env".Equals(dataItems[2]))
            {
                ServiceClient.Send(sb.ToString());
                return;
            }

            int environmentId = int.Parse(dataItems[3]);

            FingerprintingDao dao = new FingerprintingDao();
            List<Fingerprinting> fingerprintings = dao.GetFingerprinting(environmentId);

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(fingerprintings);
            ServiceClient.Send(json);
        }
    }
}
