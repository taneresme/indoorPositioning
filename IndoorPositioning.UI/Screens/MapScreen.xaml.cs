using IndoorPositioning.UI.Client;
using IndoorPositioning.UI.KNN;
using IndoorPositioning.UI.Model;
using IndoorPositioning.UI.VisualItems;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Environment = IndoorPositioning.UI.Model.Environment;

namespace IndoorPositioning.UI.Screens
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class MapScreen : UserControl, INotifyPropertyChanged, IDisposable
    {
        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    PositioningActivated = false;
                    Initialized -= MapScreen_Initialized;
                    Loaded -= MapScreen_Loaded;

                    KillPositioningThread();
                }

                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support

        /* Used to figure out whether the component loaded or not */
        private bool initialized = false;
        private Thread positioningThread;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        /* Stores selected index of the environment combobox */
        private static int selectedEnvironmentIndex = 0;
        public int SelectedEnvironmentIndex
        {
            get { return selectedEnvironmentIndex; }
            set
            {
                selectedEnvironmentIndex = value;
                OnPropertyChanged("SelectedEnvironmentIndex");
            }
        }

        /* Stores selected index of the algorithm combobox */
        private static int selectedAlgorithmIndex = 0;
        public int SelectedAlgorithmIndex
        {
            get { return selectedAlgorithmIndex; }
            set
            {
                selectedAlgorithmIndex = value;
                OnPropertyChanged("SelectedAlgorithmIndex");
            }
        }

        /* Stores selected index of the beacon combobox */
        private static int selectedBeaconIndex = 0;
        public int SelectedBeaconIndex
        {
            get { return selectedBeaconIndex; }
            set
            {
                selectedBeaconIndex = value;
                OnPropertyChanged("SelectedBeaconIndex");
            }
        }

        /* Stores selected index of the environment combobox */
        private bool positioningActivated = false;
        public bool PositioningActivated
        {
            get { return positioningActivated; }
            set
            {
                /* Check the inputs */
                if (value && (selectedBeaconIndex == -1 ||
                    selectedEnvironmentIndex == -1 ||
                    selectedAlgorithmIndex == -1))
                {
                    MessageBox.Show("Please do the selections!");
                    return;
                }
                positioningActivated = value;
                OnPropertyChanged("PositioningActivated");

                if (positioningActivated)
                {
                    StartPositioning();
                }
                else
                {
                    KillPositioningThread();
                }
            }
        }

        public bool ShowLoadingScreen
        {
            set
            {
                grdPleaseWait.Visibility = value
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        public MapScreen()
        {
            Initialized += MapScreen_Initialized;
            Loaded += MapScreen_Loaded;

            InitializeComponent();

            DataContext = this;
        }

        private void MapScreen_Loaded(object sender, RoutedEventArgs e)
        {
            if (!initialized) return;
            Load();
        }

        private void MapScreen_Initialized(object sender, EventArgs e)
        {
            initialized = true;
        }

        private void KillPositioningThread()
        {
            try { positioningThread.Abort(); }
            catch { }
            try { positioningThread = null; }
            catch { }
        }

        /* Load the list of environments from Server */
        private void Load()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                cbEnvironments.ItemsSource = IndoorPositioningClient.GetEnvironments();
                cbBeacons.ItemsSource = IndoorPositioningClient.GetBeacons();
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                MessageBox.Show(ex.ToString());
            }
        }

        private void StartPositioning()
        {
            try
            {
                /* Change server mode into positioning */
                Beacon beacon = (Beacon)cbBeacons.SelectedItem;
                IndoorPositioningClient.SetModeAsPositioning(beacon.BeaconId);

                environment = (Environment)cbEnvironments.SelectedItem;

                positioningThread = new Thread(Positioning)
                {
                    IsBackground = true
                };
                positioningThread.Start();

                ShowLoadingScreen = true;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private Environment environment;
        private void Positioning()
        {
            try
            {
                /* Load fingerprinting data from the server */
                List<AdjustedFingerprinting> fingerprintings = IndoorPositioningClient.GetFingerprintings(environment.EnvironmentId);
                /* Disappear the loading splash screen */
                Application.Current.Dispatcher.Invoke(() => ShowLoadingScreen = false);

                /* After fetching and processing the fingerprinting data, I am able to get the class count.
                 * Basically, each of the reference point is a class to be classified. */
                int numClasses = IndoorPositioningClient.GetPoints(environment.EnvironmentId).Count;
                int gatewayCount = fingerprintings[0].RssiValueAndGateway.Count;

                /* run the knn classifier on the data */
                KnnClassifier classifier = new KnnClassifier();
                while (PositioningActivated)
                {
                    /* get the Rssi values of the beacon in question from the server */
                    AdjustedFingerprinting rssi = IndoorPositioningClient.GetRssi(gatewayCount);

                    /* we will use also gateway count on the area as K constant */
                    Coordinate coordinate = classifier.Classify(rssi, fingerprintings, numClasses, gatewayCount);

                    /* Add the beacon localized onto the screen */
                    Application.Current.Dispatcher.Invoke(() => environmentShape.MoveBeacon(coordinate.Xaxis, coordinate.Yaxis));

                    /* Every second, query the server for the RSSI values read by the gateways */
                    Thread.Sleep(1000);
                }
            }
            catch (ThreadAbortException) { /* do nothing */ }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => txtAlert.Text = ex.Message);
            }
        }
    }
}
