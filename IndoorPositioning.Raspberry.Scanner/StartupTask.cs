using System;
using Windows.ApplicationModel.Background;
using System.Diagnostics;
using Windows.Devices.Bluetooth.Advertisement;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace IndoorPositioning.Raspberry.Scanner
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BluetoothLEAdvertisementWatcher _watcher;
        private string _logFile = @"C:\back-ground-te.log";
        private string _apiAddress = "http://192.168.1.36:9999/api/logger";

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            try
            {
                AppendLine("Application Started");
                // 
                // TODO: Insert code to perform background work
                //
                // If you start any asynchronous methods here, prevent the task
                // from closing prematurely by using BackgroundTaskDeferral as
                // described in http://aka.ms/backgroundtaskdeferral
                //

                _watcher = new BluetoothLEAdvertisementWatcher();
                _watcher.Received += _watcher_Received;
                _watcher.Stopped += _watcher_Stopped;
                _watcher.Start();
            }
            catch (Exception ex)
            {
                AppendLine("EXCEPTION: " + ex.ToString());
            }
        }

        private async void AppendLine(string text)
        {
            string log = "{\"log\":\"" + DateTime.Now.ToString() + ":" + text + "\"}";

            Debug.WriteLine(log);

            //HttpClient client = new HttpClient();
            //HttpContent content = new StringContent(log, Encoding.UTF8, "application/json");
            //await client.PostAsync(_apiAddress, content);

            //File.AppendAllText(_logFile, log);
        }

        private void _watcher_Stopped(BluetoothLEAdvertisementWatcher sender,
            BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            AppendLine("Watcher stopped: " + args.Error);
        }

        private void _watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            AppendLine("Received: ");
            AppendLine("  BluetoothAddress      : " + args.BluetoothAddress);
            AppendLine("  RawSignalStrengthInDBm: " + args.RawSignalStrengthInDBm);
            AppendLine("  AdvertisementType     : " + args.AdvertisementType);
            AppendLine("  ScanningMode          : " + sender.ScanningMode);
        }
    }
}
