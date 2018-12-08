using IndoorPositioning.UI.Client;
using IndoorPositioning.UI.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace IndoorPositioning.UI.Components
{
    /// <summary>
    /// Interaction logic for Gateways.xaml
    /// </summary>
    public partial class GatewaysScreen : UserControl
    {
        private bool initialized = false;

        public GatewaysScreen()
        {
            Initialized += GatewaysScreen_Initialized;
            Loaded += GatewaysScreen_Loaded;

            InitializeComponent();
        }

        private void GatewaysScreen_Loaded(object sender, RoutedEventArgs e)
        {
            if (!initialized) return;
            Load();
        }

        private void GatewaysScreen_Initialized(object sender, EventArgs e)
        {
            initialized = true;
        }

        private void Load()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                lstGateways.ItemsSource = IndoorPositioningClient.GetGateways();
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                MessageBox.Show(ex.ToString());
            }
        }

        private void lstGateways_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstGateways.SelectedItem == null) { return; }
            Gateway gateway = (Gateway)lstGateways.SelectedItem;

            txtGatewayName.Text = gateway.Name;
            txtGatewayMacAddress.Text = gateway.MACAddress;
            txtGatewayLastSignalTime.Text = gateway.LastSignalTimestamp.ToString();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                Gateway gateway = (Gateway)lstGateways.SelectedItem;
                gateway.Name = txtGatewayName.Text;
                IndoorPositioningClient.UpdateGateway(gateway);

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
