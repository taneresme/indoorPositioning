using IndoorPositioning.UI.Client;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace IndoorPositioning.UI.Screens
{
    /// <summary>
    /// Interaction logic for FingerprintingScreen.xaml
    /// </summary>
    public partial class FingerprintingScreen : UserControl, INotifyPropertyChanged
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

        /* Stores selected index of the environment combobox */
        private bool fingerprintingActivated = false;
        public bool FingerprintingActivated
        {
            get { return fingerprintingActivated; }
            set
            {
                fingerprintingActivated = value;
                OnPropertyChanged("FingerprintingActivated");
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
