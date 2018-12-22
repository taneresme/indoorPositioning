using IndoorPositioning.UI.Client;
using IndoorPositioning.UI.Model;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Environment = IndoorPositioning.UI.Model.Environment;

namespace IndoorPositioning.UI.Screens
{
    /// <summary>
    /// Interaction logic for FingerprintingScreen.xaml
    /// </summary>
    public partial class FingerprintingScreen : UserControl, INotifyPropertyChanged, IDisposable
    {
        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    FingerprintingActivated = false;
                    Initialized -= MapScreen_Initialized;
                    Loaded -= MapScreen_Loaded;
                }

                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        /* Used to figure out whether the component loaded or not */
        private bool initialized = false;

        /* Stores selected index of the environment combobox */
        private static int selectedEnvironmentIndex = -1;
        public int SelectedEnvironmentIndex
        {
            get { return selectedEnvironmentIndex; }
            set
            {
                selectedEnvironmentIndex = value;
                OnPropertyChanged("SelectedEnvironmentIndex");
            }
        }

        /* Stores selected index of the beacon combobox */
        private static int selectedBeaconIndex = -1;
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
        private bool fingerprintingActivated = false;
        public bool FingerprintingActivated
        {
            get { return fingerprintingActivated; }
            set
            {
                /* Check the inputs */
                if(value && (selectedBeaconIndex == -1 || selectedEnvironmentIndex == -1))
                {
                    MessageBox.Show("Please do the selections!");
                    return;
                }
                fingerprintingActivated = value;
                /* Change the server fingerprinting mode */
                /* Fingerprinting is activated when a reference point is selected,
                 * not here */
                if (!fingerprintingActivated)
                {
                    DeactivateFingerprinting();
                    txtAlert.Text = "";
                }
                else
                {
                    txtAlert.Text = "As soon as you select a reference point on the map, fingerprinting will be activated";
                }
                
                OnPropertyChanged("FingerprintingActivated");
            }
        }

        /* This property shows the status of the fingerprinting on the server */
        private SolidColorBrush fingerprintingBrush = new SolidColorBrush(Colors.Red);
        public SolidColorBrush FingerprintingBrush
        {
            get { return fingerprintingBrush; }
            set
            {
                fingerprintingBrush = value;
                OnPropertyChanged("FingerprintingBrush");
            }
        }

        public FingerprintingScreen()
        {
            Initialized += MapScreen_Initialized;
            Loaded += MapScreen_Loaded;

            InitializeComponent();

            DataContext = this;
        }

        private void MapScreen_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!initialized) return;
            Load();
        }

        private void MapScreen_Initialized(object sender, System.EventArgs e)
        {
            initialized = true;
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

        private void ActivateFingerprinting()
        {
            try
            {
                /* Send the mode changing request to the server */
                Environment env = (Environment)cbEnvironments.SelectedItem;
                Beacon beacon = (Beacon)cbBeacons.SelectedItem;
                IndoorPositioningClient.SetModeAsFingerprinting(beacon.BeaconId, env.EnvironmentId, 
                    environmentShape.SelectedXaxis, environmentShape.SelectedYaxis);

                /* Change the color of the fingerprinting ellipse */
                FingerprintingBrush = new SolidColorBrush(Colors.Green);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        /* */
        private void DeactivateFingerprinting()
        {
            try
            {
                IndoorPositioningClient.SetModeAsIdle();

                /* Change the color of the fingerprinting ellipse */
                FingerprintingBrush = new SolidColorBrush(Colors.Red);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        /* This event runs when the selected reference point on the map changed */
        private void environmentShape_SelectedReferencePointChanged(int xaxis, int yaxis)
        {
            /* Check the status of the UI application status to start fingerprinting */
            if (!FingerprintingActivated) return;
            /* If the fingerprinting already active on server and
             * the selected reference point changed before deactivating
             * fingerprinting then we are deactivating it and showing a message to
             * inform the user*/
            if (FingerprintingBrush.Color == Colors.Green)
            {
                DeactivateFingerprinting();
                MessageBox.Show("Fingerprinting deactivated! You first complete " +
                    "the fingerprinting on the prior reference point and then move on the new one.");
                return;
            }
            ActivateFingerprinting();
        }

        /* This event runs when the all selected reference point removed/unselected */
        private void environmentShape_AllReferencePointsUnselected()
        {
            DeactivateFingerprinting();
        }
    }
}
