using IndoorPositioning.UI.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Environment = IndoorPositioning.UI.Model.Environment;

namespace IndoorPositioning.UI.Client
{
    public class IndoorPositioningClient
    {
        private const int RECEIVE_TIMEOUT = 10; //sn

        private const string GET_GATEWAYS_COMMAND = "get gateways";
        private const string GET_BEACONS_COMMAND = "get beacons";
        private const string GET_ENVIRONMENTS_COMMAND = "get environments";

        private const string UPDATE_GATEWAY_COMMAND = "update gateway ";
        private const string UPDATE_BEACON_COMMAND = "update beacon ";
        private const string UPDATE_ENVIRONMENT_COMMAND = "update environment ";

        private const string DELETE_GATEWAY_COMMAND = "delete gateway ";
        private const string DELETE_BEACON_COMMAND = "delete beacon ";
        private const string DELETE_ENVIRONMENT_COMMAND = "delete environment ";

        private const string ADD_ENVIRONMENT_COMMAND = "add environment ";

        private const string SET_MODE_COMMAND = "set mode ";

        #region COMMON METHODS

        private static string Get(string command)
        {
            TcpConsumer.Instance.Send(command);
            string json = TcpConsumer.Instance.Receive(RECEIVE_TIMEOUT);

            if (json.StartsWith("error"))
            {
                throw new Exception(json);
            }

            return json;
        }

        private static void Post(string command)
        {
            TcpConsumer.Instance.Send(command);
            string result = TcpConsumer.Instance.Receive(RECEIVE_TIMEOUT);

            if (result.StartsWith("error"))
            {
                throw new Exception(result);
            }
        }

        #endregion COMMON METHODS

        #region GATEWAY METHODS

        /* gets gateways from server */
        public static List<Gateway> GetGateways()
        {
            string json = Get(GET_GATEWAYS_COMMAND);

            List<Gateway> gateways = JsonConvert.DeserializeObject<List<Gateway>>(json);
            return gateways;
        }

        /* sends the update-gateway request to the server */
        public static void UpdateGateway(Gateway gateway)
        {
            string json = JsonConvert.SerializeObject(gateway);

            Post(UPDATE_GATEWAY_COMMAND + json);
        }

        /* sends the delete-gateway request to the server */
        public static void DeleteGateway(Gateway gateway)
        {
            string json = JsonConvert.SerializeObject(gateway);

            Post(DELETE_GATEWAY_COMMAND + json);
        }

        #endregion GATEWAY METHODS

        #region BEACON METHODS

        /* gets beacons from server */
        public static List<Beacon> GetBeacons()
        {
            string json = Get(GET_BEACONS_COMMAND);

            List<Beacon> beacons = JsonConvert.DeserializeObject<List<Beacon>>(json);
            return beacons;
        }

        /* sends the update-beacon request to the server */
        public static void UpdateBeacon(Beacon beacon)
        {
            string json = JsonConvert.SerializeObject(beacon);

            Post(UPDATE_BEACON_COMMAND + json);
        }

        /* sends the delete-beacon request to the server */
        public static void DeleteBeacon(Beacon beacon)
        {
            string json = JsonConvert.SerializeObject(beacon);

            Post(DELETE_BEACON_COMMAND + json);
        }

        #endregion BEACON METHODS

        #region ENVIRONMENT METHODS

        /* gets environment definitions from server */
        public static List<Environment> GetEnvironments()
        {
            string json = Get(GET_ENVIRONMENTS_COMMAND);

            List<Environment> environments = JsonConvert.DeserializeObject<List<Environment>>(json);
            return environments;
        }

        /* sends the update-environment request to the server */
        public static void UpdateEnvironment(Environment environment)
        {
            string json = JsonConvert.SerializeObject(environment);

            Post(UPDATE_ENVIRONMENT_COMMAND + json);
        }

        /* sends the delete-environment request to the server */
        public static void DeleteEnvironment(Environment environment)
        {
            string json = JsonConvert.SerializeObject(environment);

            Post(DELETE_ENVIRONMENT_COMMAND + json);
        }

        /* sends the add-environment request to the server */
        public static void AddEnvironment(Environment environment)
        {
            string json = JsonConvert.SerializeObject(environment);

            Post(ADD_ENVIRONMENT_COMMAND + json);
        }

        #endregion ENVIRONMENT METHODS

        #region MODE METHODS

        /* sends the command to set the server mode as positioning */
        public static void SetModeAsPositioning()
        {
            Post(SET_MODE_COMMAND + "positioning");
        }

        /* sends the command to set the server mode as fingerprinting */
        public static void SetModeAsFingerprinting(int environmentId, int x, int y)
        {
            Post(SET_MODE_COMMAND + string.Format($"fingerprinting -env {environmentId} -x {x} -y {y}"));
        }

        #endregion MODE METHODS


    }
}
