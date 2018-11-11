using System;
using Windows.Devices.Bluetooth.Advertisement;

// This example code shows how you could implement the required main function for a 
// Console UWP Application. You can replace all the code inside Main with your own custom code.

// You should also change the Alias value in the AppExecutionAlias Extension in the 
// Package.appxmanifest to a value that you define. To edit this file manually, right-click
// it in Solution Explorer and select View Code, or open it with the XML Editor.

namespace IndoorPositioning.Raspberry.Scanner
{
    class Program
    {
        private static BluetoothLEAdvertisementWatcher _watcher;

        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("Hello - no args");
                }
                else
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        Console.WriteLine($"arg[{i}] = {args[i]}");
                    }
                }

                _watcher = new BluetoothLEAdvertisementWatcher();
                _watcher.Received += _watcher_Received;
                _watcher.Stopped += _watcher_Stopped;
                _watcher.Start();
                Console.WriteLine("Scanning started...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            Console.ReadLine();
        }

        private static void _watcher_Stopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            Console.WriteLine("Watcher stopped: ");
        }

        private static void _watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            Console.WriteLine("Received: ");
            Console.WriteLine("  BluetoothAddress: " + args.BluetoothAddress);
        }
    }
}
