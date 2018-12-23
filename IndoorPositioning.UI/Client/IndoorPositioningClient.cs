using IndoorPositioning.UI.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private const string GET_FINGERINTING_COMMAND = "get fingerprinting";

        private const string GET_RSSI_COMMAND = "get rssi";

        /* Stores the list of fingerprinting with the corresponding environment Id */
        private static Dictionary<int, List<AdjustedFingerprinting>> fingerprintings =
            new Dictionary<int, List<AdjustedFingerprinting>>();

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

        /* sends the command to set the server mode as idle */
        public static void SetModeAsIdle()
        {
            Post(SET_MODE_COMMAND + "idle");
        }

        /* sends the command to set the server mode as fingerprinting */
        public static void SetModeAsFingerprinting(int beaconId, int environmentId, int x, int y)
        {
            Post(SET_MODE_COMMAND + string.Format($"fingerprinting -env {environmentId} -beacon {beaconId} -x {x} -y {y}"));
        }

        /* sends the command to set the server mode as positioning */
        public static void SetModeAsPositioning(int beaconId)
        {
            Post(SET_MODE_COMMAND + string.Format($"positioning -beacon {beaconId}"));
        }

        #endregion MODE METHODS

        #region POSITIONING METHODS

        /* In the KnnClassifier class, I will need to count the total hit counts
         * of the point according to the K. Here I am trying to create the point objects
         * of the reference points only once. In this way, I will be able to access same 
         * point objects through the different fingerprints. Thanks to this approach,
         * In KNNclassifier, I will be able to count the hits of the points by using the points 
         * themselves without using any other structure (array and so).
         */
        private static Dictionary<int, List<Coordinate>> points = new Dictionary<int, List<Coordinate>>();
        private static Coordinate GetPoint(int environmentId, int xaxis, int yaxis)
        {
            /* First control if the environment registered.
             * At this first time of the method called, there will be no
             * environment in the dictionary
             */
            if (!points.ContainsKey(environmentId))
            {
                points.Add(environmentId, new List<Coordinate>());
            }

            /* get the list of the points and check them */
            List<Coordinate> axises = points[environmentId];
            foreach (var axis in axises)
            {
                if (axis.Xaxis == xaxis && axis.Yaxis == yaxis)
                {
                    return axis;
                }
            }
            /* If we could not found the point in the list
             * we will add it to the list to use them in the next call */
            Coordinate point = new Coordinate()
            {
                Xaxis = xaxis,
                Yaxis = yaxis
            };
            axises.Add(point);
            return point;
        }

        /* Returns the list of the points created during the processing fingerprinting data */
        public static List<Coordinate> GetPoints(int environmentId)
        {
            if (!points.ContainsKey(environmentId))
            {
                return new List<Coordinate>();
            }
            /* In every single request, we are setting the hit count as zero.
             * Because this variable is static and causes problem in the further requests */
            foreach (var item in points[environmentId])
            {
                item.HitCount = 0;
            }
            return points[environmentId];
        }

        /* sends the command to fetch fingerprinting data from the server*/
        public static List<AdjustedFingerprinting> GetFingerprintings(int environmentId)
        {
            /* If we fetch the fingerprintings of the environment in question.
             * we do not call the server again, return them from the cache */
            if (!fingerprintings.ContainsKey(environmentId))
            {
                /* Fetch the fingerprintings from the server and cache. */
                string json = Get(string.Format($"{GET_FINGERINTING_COMMAND} -env {environmentId}"));
                List<Fingerprinting> listOfFingerprinting = JsonConvert.DeserializeObject<List<Fingerprinting>>(json);

                /* Create a new cache with the environment Id */
                List<AdjustedFingerprinting> adjustedFingerprintings = new List<AdjustedFingerprinting>();
                fingerprintings.Add(environmentId, adjustedFingerprintings);

                /* There is an assumption below that is all the data will come in sorted manner.
                 * The data should be sorted X-axis, Y-axis, timestamp, and gateway Id.
                 * This sorting should be giving a list like:
                 * ------ x_axis -- y_axis -- timestamp -- gateway_id
                 * ------ 40     -- 40     -- 2018-12.. -- 1
                 * ------ 40     -- 40     -- 2018-12.. -- 2
                 * ------ 40     -- 40     -- 2018-12.. -- 3
                 * ------ 40     -- 40     -- 2018-12.. -- 1
                 * ------ 40     -- 40     -- 2018-12.. -- 2
                 * ------ 40     -- 40     -- 2018-12.. -- 3
                 */
                AdjustedFingerprinting adjustedFingerprinting = new AdjustedFingerprinting();
                /* I am going to use this value to get that I completed to process all gateways
                 * for each of the points */
                int firstGateway = listOfFingerprinting[0].GatewayId;
                /* Rssi values fetched by each of the gateways will be stored on this variable.
                 * As soon as the gateway changes, I will be re-initializing this variable */
                RssiValue rssiValue = new RssiValue();

                for (int i = 0; i < listOfFingerprinting.Count; i++)
                {
                    /* gateway changed */
                    if (listOfFingerprinting[i].GatewayId == firstGateway)
                    {
                        /* If the gateway is the first gateway of the list
                         * this means all gateways processed for the corresponding
                         * reference point and it is time to refresh */
                        adjustedFingerprinting = new AdjustedFingerprinting()
                        {
                            /* Get one of the existing points */
                            Coordinates = GetPoint(environmentId, 
                                listOfFingerprinting[i].Xaxis,
                                listOfFingerprinting[i].Yaxis)
                        };
                        adjustedFingerprintings.Add(adjustedFingerprinting);
                    }
                    /* re-initialize the rssiValue variable */
                    rssiValue = new RssiValue()
                    {
                        GatewayId = listOfFingerprinting[i].GatewayId,
                        Rssi = (listOfFingerprinting[i].Rssi)
                    };
                    adjustedFingerprinting.RssiValueAndGateway.Add(rssiValue);
                }
            }

            return fingerprintings[environmentId];
        }

        /* gets current rssi values of the beacon provided with command of set mode positioning */
        public static RssiValue[] GetRssi(int count)
        {
            string json = Get(string.Format($"{GET_RSSI_COMMAND} -count {count}"));

            Debug.WriteLine(json);

            RssiValue[] rssiValues = JsonConvert.DeserializeObject<RssiValue[]>(json);
            foreach (var rssi in rssiValues)
            {
                rssi.Rssi = (rssi.Rssi);
            }

            return rssiValues;
        }

        #endregion POSITIONING METHODS
    }
}
