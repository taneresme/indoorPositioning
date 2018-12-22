using IndoorPositioning.Server.Clients;
using IndoorPositioning.Server.Database.Dao;
using IndoorPositioning.Server.Database.Model;
using IndoorPositioning.Server.Static;
using System.Text;

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

        /* Creates error message for set mode command */
        private string CreateError_SetMode()
        {
            /* Create error message */
            StringBuilder sb = new StringBuilder()
                .AppendLine(INVALID_PARAMETERS_ERROR)
                .AppendLine("Sample: set mode idle")
                .AppendLine("Sample: set mode positioning -beacon 2")
                .AppendLine("Sample: set mode fingerprinting -env 2 -beacon 3 -x 10 -y 20")
                .AppendLine("-env: environment id to be fingerprinted")
                .AppendLine("-beacon: beacon id to be used for fingerprinting")
                .AppendLine("-x: x axis")
                .AppendLine("-y: y axis");
            foreach (var item in ServerSettings.Modes)
            {
                sb.AppendLine(string.Format($"{item.Key} for {item.Value}"));
            }

            return sb.ToString();
        }

        private int GetIndex(string[] dataItems, string p)
        {
            for (int i = 0; i < dataItems.Length; i++)
            {
                if (dataItems[i].Equals(p)) return i;
            }
            return -1;
        }

        /* Updates the beacon that is provided as json */
        private void SetMode(string[] dataItems)
        {
            /* Check data */
            if (dataItems.Length < 3)
            {
                ServiceClient.Send(CreateError_SetMode());
                return;
            }

            string mode = dataItems[2];

            /* If mode string is invalid */
            if (!ServerSettings.Modes.ContainsKey(mode))
            {
                ServiceClient.Send(CreateError_SetMode());
                return;
            }

            /* If fingerprinting mode is being tried to set */
            if (ServerSettings.Modes[mode] == Enums.ServerModes.Fingerprinting)
            {
                /* Check the command whether it is valid for the fingerprinting */
                if (dataItems.Length < 11)
                {
                    ServiceClient.Send(CreateError_SetMode());
                    return;
                }

                /* Check the parameters */
                int envIndex = GetIndex(dataItems, "-env");
                if (envIndex == -1)
                {
                    ServiceClient.Send(CreateError_SetMode());
                    return;
                }
                int beaconIndex = GetIndex(dataItems, "-beacon");
                if (beaconIndex == -1)
                {
                    ServiceClient.Send(CreateError_SetMode());
                    return;
                }
                int xIndex = GetIndex(dataItems, "-x");
                if (xIndex == -1)
                {
                    ServiceClient.Send(CreateError_SetMode());
                    return;
                }
                int yIndex = GetIndex(dataItems, "-y");
                if (yIndex == -1)
                {
                    ServiceClient.Send(CreateError_SetMode());
                    return;
                }
                /* Exceptions will be handled by ServiceClient */
                int env = int.Parse(dataItems[envIndex + 1]);
                int beaconId = int.Parse(dataItems[beaconIndex + 1]);
                int x = int.Parse(dataItems[xIndex + 1]);
                int y = int.Parse(dataItems[yIndex + 1]);

                /* Get the mac address of the beacon provided! */
                BeaconDao dao = new BeaconDao();
                Beacon beacon = dao.GetBeaconById(beaconId);

                /*Set coordinates*/
                FingerprintingSettings.Fingerprinting_EnvironmentId = env;
                FingerprintingSettings.Fingerprinting_BeaconId = beaconId;
                FingerprintingSettings.Fingerprinting_BeaconMacAddress = beacon.MACAddress;
                FingerprintingSettings.Fingerprinting_X = x;
                FingerprintingSettings.Fingerprinting_Y = y;
            }
            else if (ServerSettings.Modes[mode] == Enums.ServerModes.Positioning)
            {
                /* Check the command whether it is valid for positioning */
                if (dataItems.Length < 5)
                {
                    ServiceClient.Send(CreateError_SetMode());
                    return;
                }
                /* Check the parameters */
                int beaconIndex = GetIndex(dataItems, "-beacon");
                if (beaconIndex == -1)
                {
                    ServiceClient.Send(CreateError_SetMode());
                    return;
                }

                int beaconId = int.Parse(dataItems[beaconIndex + 1]);
                /* Get the mac address of the beacon provided! */
                BeaconDao dao = new BeaconDao();
                Beacon beacon = dao.GetBeaconById(beaconId);

                /* Set the beacon to be positioned */
                PositioningParams.Positioning_BeaconId = beaconId;
                PositioningParams.Positioning_BeaconMacAddress = beacon.MACAddress;
            }

            /* Set mode */
            ServerSettings.ServerMode = ServerSettings.Modes[mode];
            ServiceClient.Send(OK);
        }
    }
}
