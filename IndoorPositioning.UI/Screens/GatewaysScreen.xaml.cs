using IndoorPositioning.UI.Client;
using IndoorPositioning.UI.Converters;
using IndoorPositioning.UI.Model;
using IndoorPositioning.UI.VisualItems;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace IndoorPositioning.UI.Screens
{
    /// <summary>
    /// Interaction logic for Gateways.xaml
    /// </summary>
    public partial class GatewaysScreen : UserControl
    {
        private bool initialized = false;
        private Gateway selectedGateway;
        private int selectedIndex = 0;

        public GatewaysScreen()
        {
            Initialized += GatewaysScreen_Initialized;
            Loaded += GatewaysScreen_Loaded;

            InitializeComponent();

            SizeChanged += GatewaysScreen_SizeChanged;
        }

        private void GatewaysScreen_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            /* If the size of the screen changes, Gateway in the canvvas will be rendered again. */
            PutGatewayIntoCanvas();
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
                if (lstGateways.Items.Count > 0) { lstGateways.SelectedIndex = selectedIndex; }
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
            selectedGateway = (Gateway)lstGateways.SelectedItem;
            selectedIndex = lstGateways.SelectedIndex;

            gridGatewayDetails.DataContext = selectedGateway;

            PutGatewayIntoCanvas();
        }

        private void PutGatewayIntoCanvas()
        {
            if (selectedGateway == null) { return; }

            /* Put gateway shape */
            GatewayShape shape = new GatewayShape();
            /* We are clearing all old items */
            canvasGatewayPosition.Children.Clear();
            canvasGatewayPosition.Children.Add(shape);

            /* We are setting the y-axis value
             * If the Yaxis value is O, the distance from left will be 0
             * If the Yaxis value is N, the distance from left will be the width of the canvas 
             * If the Yaxis value is N/2, the distance from left will be the half of the width of the canvas
             * The converter class does the action above */
            Canvas.SetLeft(shape,
                (double)new GatewayPositionToCanvasCoordinateConverter()
                .Convert(
                    new object[] {
                        selectedGateway.Yaxis,
                        canvasGatewayPosition.ActualHeight,
                        canvasGatewayPosition.ActualWidth
                    }, null, "width", null));

            /* We are setting the x-axis value
             * If the Xaxis value is O, the distance from top will be 0
             * If the Xaxis value is N, the distance from top will be the height of the canvas 
             * If the Xaxis value is N/2, the distance from top will be the half of the height of the canvas
             * The converter class does the action above */
            Canvas.SetTop(shape,
                (double)new GatewayPositionToCanvasCoordinateConverter()
                .Convert(
                    new object[] {
                        selectedGateway.Xaxis,
                        canvasGatewayPosition.ActualHeight,
                        canvasGatewayPosition.ActualWidth
                    }, null, "height", null));
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGateway == null) { return; }

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                IndoorPositioningClient.UpdateGateway(selectedGateway);

                /* Load the list again */
                Load();
                /* Select the same item from the list. */
                lstGateways.SelectedIndex = selectedIndex;
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
            if (selectedGateway == null) { return; }

            MessageBoxResult result = MessageBox.Show("Are you sure to delete this?", "WARNING!", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No) { return; }

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                Gateway gateway = (Gateway)lstGateways.SelectedItem;
                IndoorPositioningClient.DeleteGateway(gateway);

                /* remove selected item */
                selectedGateway = null;
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
