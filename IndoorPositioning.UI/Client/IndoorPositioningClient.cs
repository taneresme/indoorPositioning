using IndoorPositioning.UI.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace IndoorPositioning.UI.Client
{
    public class IndoorPositioningClient
    {
        private const int RECEIVE_TIMEOUT = 10; //sn
        private const string GET_GATEWAYS_COMMAND = "get gateways";
        private const string GET_BEACONS_COMMAND = "get beacons";
        private const string UPDATE_GATEWAY_COMMAND = "update gateway ";
        private const string UPDATE_BEACON_COMMAND = "update beacon ";
        private const string DELETE_GATEWAY_COMMAND = "delete gateway ";
        private const string DELETE_BEACON_COMMAND = "delete beacon ";

        /* gets gateways from server */
        public static List<Gateway> GetGateways()
        {
            TcpConsumer.Instance.Send(GET_GATEWAYS_COMMAND);
            string json = TcpConsumer.Instance.Receive(RECEIVE_TIMEOUT);

            if (json.StartsWith("error"))
            {
                throw new Exception(json);
            }

            List<Gateway> gateways = JsonConvert.DeserializeObject<List<Gateway>>(json);
            return gateways;

        }

        /* gets beacons from server */
        public static List<Beacon> GetBeacons()
        {
            TcpConsumer.Instance.Send(GET_BEACONS_COMMAND);
            string json = TcpConsumer.Instance.Receive(RECEIVE_TIMEOUT);

            if (json.StartsWith("error"))
            {
                throw new Exception(json);
            }

            List<Beacon> beacons = JsonConvert.DeserializeObject<List<Beacon>>(json);
            return beacons;
        }

        /* sends the update-gateway request to the server */
        public static void UpdateGateway(Gateway gateway)
        {
            string json = JsonConvert.SerializeObject(gateway);

            TcpConsumer.Instance.Send(UPDATE_GATEWAY_COMMAND + json);
            string result = TcpConsumer.Instance.Receive(RECEIVE_TIMEOUT);

            if (result.StartsWith("error"))
            {
                throw new Exception(json);
            }
        }

        /* sends the update-beacon request to the server */
        public static void UpdateBeacon(Beacon beacon)
        {
            string json = JsonConvert.SerializeObject(beacon);

            TcpConsumer.Instance.Send(UPDATE_BEACON_COMMAND + json);
            string result = TcpConsumer.Instance.Receive(RECEIVE_TIMEOUT);

            if (result.StartsWith("error"))
            {
                throw new Exception(json);
            }
        }

        /* sends the delete-gateway request to the server */
        public static void DeleteGateway(Gateway gateway)
        {
            string json = JsonConvert.SerializeObject(gateway);

            TcpConsumer.Instance.Send(DELETE_GATEWAY_COMMAND + json);
            string result = TcpConsumer.Instance.Receive(RECEIVE_TIMEOUT);

            if (result.StartsWith("error"))
            {
                throw new Exception(json);
            }
        }

        /* sends the delete-beacon request to the server */
        public static void DeleteBeacon(Beacon beacon)
        {
            string json = JsonConvert.SerializeObject(beacon);

            TcpConsumer.Instance.Send(DELETE_BEACON_COMMAND + json);
            string result = TcpConsumer.Instance.Receive(RECEIVE_TIMEOUT);

            if (result.StartsWith("error"))
            {
                throw new Exception(json);
            }
        }

    }
}
