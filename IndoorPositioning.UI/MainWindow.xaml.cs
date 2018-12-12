using IndoorPositioning.UI.Screens;
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
            var screen = new GatewaysScreen();
            content.Content = screen;
        }

        private void mnItemBeacons_Click(object sender, RoutedEventArgs e)
        {
            var screen = new BeaconsScreen();
            content.Content = screen;
        }

        private void mnItemEnvironments_Click(object sender, RoutedEventArgs e)
        {
            var screen = new EnvironmentsScreen();
            content.Content = screen;
        }

        private void mnItemMap_Click(object sender, RoutedEventArgs e)
        {
            var screen = new MapScreen();
            content.Content = screen;
        }
    }
}
