using IndoorPositioning.UI.Client;
using IndoorPositioning.UI.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace IndoorPositioning.UI.Components
{
    /// <summary>
    /// Interaction logic for Beacons.xaml
    /// </summary>
    public partial class BeaconsScreen : UserControl
    {
        private bool initialized = false;
        
        public BeaconsScreen()
        {
            Initialized += BeaconsScreen_Initialized;
            Loaded += BeaconsScreen_Loaded;

            InitializeComponent();
        }

        private void BeaconsScreen_Loaded(object sender, RoutedEventArgs e)
        {
            if (!initialized) return;
            Load();
        }

        private void BeaconsScreen_Initialized(object sender, EventArgs e)
        {
            initialized = true;
        }

        private void Load()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                lstBeacons.ItemsSource = IndoorPositioningClient.GetBeacons();
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                MessageBox.Show(ex.ToString());
            }
        }

        private void lstBeacons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstBeacons.SelectedItem == null) { return; }
            Beacon beacon = (Beacon)lstBeacons.SelectedItem;

            txtBeaconName.Text = beacon.Name;
            txtBeaconMacAddress.Text = beacon.MACAddress;
            txtBeaconLastSignalTime.Text = beacon.LastSignalTimestamp.ToString();
            txtBeaconType.Text = beacon.BeaconType;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                Beacon beacon = (Beacon)lstBeacons.SelectedItem;
                beacon.Name = txtBeaconName.Text;
                IndoorPositioningClient.UpdateBeacon(beacon);

                /* Load the list again */
                Load();
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
