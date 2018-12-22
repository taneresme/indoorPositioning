using IndoorPositioning.Server.Clients;
using IndoorPositioning.Server.Database.Dao;
using IndoorPositioning.Server.Database.Model;
using IndoorPositioning.Server.Static;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndoorPositioning.Server.Services
{
    public class GetService : BaseService, IService
    {
        protected const string NOT_ENOUGH_DATA_FOUND_ERROR = "error: not enough data found!";

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
            else if ("rssi".Equals(command)) GetRssi(dataItems);
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
            ServiceClient.Send(ServerSettings.ServerMode.ToString());
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

        private void GetRssi(string[] dataItems)
        {
            /* Create error message */
            StringBuilder sb = new StringBuilder()
                .AppendLine(INVALID_PARAMETERS_ERROR)
                .AppendLine("Sample: get rssi -count 3")
                .AppendLine("-count: shows the how many RSSI value wanted. ")
                .AppendLine()
                .AppendLine("If more than gateway count is provided, error occurs!")
                .AppendLine("It returns the rssi values of the beacon provided by the command set mode positioning");

            /* The parameters are valid? */
            if (dataItems.Length < 4)
            {
                ServiceClient.Send(sb.ToString());
                return;
            }

            /* Check the parameters */
            if (!"-count".Equals(dataItems[2]))
            {
                ServiceClient.Send(sb.ToString());
                return;
            }

            int count = int.Parse(dataItems[3]);

            /* ıf the count value is more than the connected gateway count, returns error */
            if (count > PositioningParams.Positioning_BeaconRssi.Count)
            {
                ServiceClient.Send(NOT_ENOUGH_DATA_FOUND_ERROR + " Consider changing parameters for RSSI!");
                return;
            }

            /* Get the RSSI values and create a string */
            RssiValue[] rssiValues = new RssiValue[count];
            for (int i = 0; i < count; i++)
            {
                rssiValues[i] = new RssiValue()
                {
                    GatewayId = PositioningParams.Positioning_BeaconRssi.Keys.ToList()[i],
                    Rssi = PositioningParams.Positioning_BeaconRssi.Values.ToList()[i]
                };
            }

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(rssiValues);
            ServiceClient.Send(json);
        }
    }

    public class RssiValue
    {
        public int GatewayId { get; set; }
        public int Rssi { get; set; }
    }
}
