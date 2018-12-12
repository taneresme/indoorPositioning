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

        /* Stores the selected item of Combobox of the list of environments */
        private Environment selectedEnvironment;
        public Environment SelectedEnvironment
        {
            get { return selectedEnvironment; }
            set
            {
                selectedEnvironment = value;
                OnPropertyChanged("SelectedEnvironment");
            }
        }

        public MapScreen()
        {
            Initialized += MapScreen_Initialized;
            Loaded += MapScreen_Loaded;

            InitializeComponent();
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
