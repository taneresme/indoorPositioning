using System;
using Windows.ApplicationModel.Background;
using System.Diagnostics;
using IndoorPositioning.Beacon.Core;
using IndoorPositioning.Beacon.Bluetooth;
using System.Net.Sockets;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace IndoorPositioning.Raspberry.Scanner
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const string NAME = "RASPBERRY";

        TcpConnector tcpConnector;

        private BackgroundTaskDeferral deferral;
        private IBeaconScanner scanner;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            try
            {
                AppendLine("Application Started\n");

                /* Connect to the server */
                tcpConnector = new TcpConnector();
                tcpConnector.Connect();

                scanner = new BeaconScanner();
                scanner.NewBeaconAdded += _scanner_NewBeaconAdded;
                scanner.BeaconUpdated += _scanner_BeaconUpdated;

                scanner.Start();
            }
            catch (Exception ex)
            {
                AppendLine("EXCEPTION: " + ex.ToString());
            }
        }

        private string CreateMessage(BeaconOperationEventArgs e)
        {
            return string.Format("{0},{1},{2},{3},", NAME, e.Beacon.Address, e.LocalAddress, e.Beacon.Rssi);
        }

        private void _scanner_BeaconUpdated(object sender, BeaconOperationEventArgs e)
        {
            Debug.WriteLine("_scanner_BeaconUpdated");
            Debug.WriteLine(e.ToString());

            tcpConnector.Send(CreateMessage(e));
        }

        private void _scanner_NewBeaconAdded(object sender, BeaconOperationEventArgs e)
        {
            Debug.WriteLine("_scanner_NewBeaconAdded");
            Debug.WriteLine(e.ToString());

            tcpConnector.Send(CreateMessage(e));
        }

        private void AppendLine(string text)
        {
            try
            {
                string log = "{\"log\":\"" + DateTime.Now.ToString() + ":" + text + "\"}";

                Debug.WriteLine(log);
            }
            catch { }
        }
    }
}
