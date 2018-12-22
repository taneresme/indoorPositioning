using IndoorPositioning.UI.Client;
using IndoorPositioning.UI.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace IndoorPositioning.UI.Screens
{
    /// <summary>
    /// Interaction logic for Beacons.xaml
    /// </summary>
    public partial class BeaconsScreen : UserControl, IDisposable
    {
        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Initialized -= BeaconsScreen_Initialized;
                    Loaded -= BeaconsScreen_Loaded;
                }

                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support

        private bool initialized = false;
        private Beacon selectedBeacon;
        private int selectedIndex = 0;

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
                if (lstBeacons.Items.Count > 0) { lstBeacons.SelectedIndex = selectedIndex; }
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

            selectedBeacon = (Beacon)lstBeacons.SelectedItem;
            selectedIndex = lstBeacons.SelectedIndex;

            gridBeaconDetails.DataContext = selectedBeacon;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBeacon == null) { return; }

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                IndoorPositioningClient.UpdateBeacon(selectedBeacon);

                /* Load the list again */
                Load();
                /* Select the same item from the list. */
                lstBeacons.SelectedIndex = selectedIndex;
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBeacon == null) { return; }
            MessageBoxResult result = MessageBox.Show("Are you sure to delete this?", "WARNING!", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No) { return; }

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                IndoorPositioningClient.DeleteBeacon(selectedBeacon);

                /* remove selected item */
                selectedBeacon = null;
                selectedIndex = 0;
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

        private void btnClearAllBeacons_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure to delete this?", "WARNING!", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No) { return; }

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                foreach (Beacon beacon in lstBeacons.Items)
                {
                    /* remove all unknown beacons from the server */
                    if ("unknown".Equals(beacon.Name.ToLower()))
                    {
                        IndoorPositioningClient.DeleteBeacon(beacon);
                    }
                }

                /* remove selected item */
                selectedBeacon = null;
                selectedIndex = 0;
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
