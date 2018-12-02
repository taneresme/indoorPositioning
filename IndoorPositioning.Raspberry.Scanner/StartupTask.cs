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
        private string _logFile = @"C:\back-ground-te.log";
        private string _apiAddress = "http://192.168.1.36:9999/api/logger";

        private BackgroundTaskDeferral deferral;
        private IBeaconScanner _scanner;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            try
            {
                AppendLine("Application Started\n");
                _scanner = new BeaconScanner();
                _scanner.NewBeaconAdded += _scanner_NewBeaconAdded;
                _scanner.BeaconUpdated += _scanner_BeaconUpdated;

                _scanner.Start();
            }
            catch (Exception ex)
            {
                AppendLine("EXCEPTION: " + ex.ToString());
            }
        }

        private void _scanner_BeaconUpdated(object sender, BeaconOperationEventArgs e)
        {
            Debug.WriteLine("_scanner_BeaconUpdated");
            Debug.WriteLine(e.ToString());
        }

        private void _scanner_NewBeaconAdded(object sender, BeaconOperationEventArgs e)
        {
            Debug.WriteLine("_scanner_NewBeaconAdded");
            Debug.WriteLine(e.ToString());
        }

        private async void AppendLine(string text)
        {
            try
            {
                string log = "{\"log\":\"" + DateTime.Now.ToString() + ":" + text + "\"}";

                Debug.WriteLine(log);

                //HttpClient client = new HttpClient();
                //HttpContent content = new StringContent(log, Encoding.UTF8, "application/json");
                //await client.PostAsync(_apiAddress, content);
            }
            catch (Exception ex)
            {

            }

            //File.AppendAllText(_logFile, log);
        }
    }
}
