using IndoorPositioning.UI.Client;
using IndoorPositioning.UI.Screens;
using System;
using System.Windows;

namespace IndoorPositioning.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void mnItemGateways_Click(object sender, RoutedEventArgs e)
        {
            /* Set the mode as positioning */
            try
            {
                IndoorPositioningClient.SetModeAsIdle();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            var screen = new GatewaysScreen();
            content.Content = screen;
        }

        private void mnItemBeacons_Click(object sender, RoutedEventArgs e)
        {
            /* Set the mode as positioning */
            try
            {
                IndoorPositioningClient.SetModeAsIdle();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            var screen = new BeaconsScreen();
            content.Content = screen;
        }

        private void mnItemEnvironments_Click(object sender, RoutedEventArgs e)
        {
            /* Set the mode as positioning */
            try
            {
                IndoorPositioningClient.SetModeAsIdle();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            var screen = new EnvironmentsScreen();
            content.Content = screen;
        }

        private void mnItemMap_Click(object sender, RoutedEventArgs e)
        {
            /* Set the mode as positioning */
            IndoorPositioningClient.SetModeAsIdle();

            var screen = new MapScreen();
            content.Content = screen;
        }

        private void mnItemFingerprinting_Click(object sender, RoutedEventArgs e)
        {
            /* Set the mode as positioning */
            try
            {
                IndoorPositioningClient.SetModeAsIdle();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            var screen = new FingerprintingScreen();
            content.Content = screen;
        }
    }
}
