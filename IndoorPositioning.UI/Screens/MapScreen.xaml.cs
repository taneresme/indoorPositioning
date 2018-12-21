using IndoorPositioning.UI.Client;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Environment = IndoorPositioning.UI.Model.Environment;

namespace IndoorPositioning.UI.Screens
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class MapScreen : UserControl, INotifyPropertyChanged
    {
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

        /* Stores selected index of the algorithm combobox */
        private static int selectedAlgorithmIndex = -1;
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
        private bool positioningActivated = false;
        public bool PositioningActivated
        {
            get { return positioningActivated; }
            set
            {
                /* Check the inputs */
                if (selectedBeaconIndex == -1 || 
                    selectedEnvironmentIndex == -1 || 
                    selectedAlgorithmIndex == -1)
                {
                    MessageBox.Show("Please do the selections!");
                    return;
                }
                positioningActivated = value;

                OnPropertyChanged("PositioningActivated");
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
    }
}
