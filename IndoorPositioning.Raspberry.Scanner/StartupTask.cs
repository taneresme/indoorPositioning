using System;
using Windows.ApplicationModel.Background;
using System.Diagnostics;
using IndoorPositioning.Beacon.Core;
using IndoorPositioning.Beacon.Bluetooth;

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
                scanner.BeaconSignalReceived += _scanner_BeaconSignalReceived;

                scanner.Start();
            }
            catch (Exception ex)
            {
                AppendLine("EXCEPTION: " + ex.ToString());
            }
        }

        private string CreateMessage(BeaconOperationEventArgs e)
        {
            return string.Format($"{NAME},{e.Beacon.Address},{e.LocalAddress},{e.Beacon.Rssi},");
        }

        private void _scanner_BeaconSignalReceived(object sender, BeaconOperationEventArgs e)
        {
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
