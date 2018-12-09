using IndoorPositioning.UI.Client;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Environment = IndoorPositioning.UI.Model.Environment;

namespace IndoorPositioning.UI.Components
{
    /// <summary>
    /// Interaction logic for EnvironmentsScreen.xaml
    /// </summary>
    public partial class EnvironmentsScreen : UserControl
    {
        private bool initialized = false;
        private Environment selectedEnvironment;
        private int selectedIndex = 0;

        public EnvironmentsScreen()
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
                lstEnvironments.ItemsSource = IndoorPositioningClient.GetEnvironments();
                if (lstEnvironments.Items.Count > 0) { lstEnvironments.SelectedIndex = selectedIndex; }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                MessageBox.Show(ex.ToString());
            }
        }

        private void lstEnvironments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstEnvironments.SelectedItem == null) { return; }

            selectedEnvironment = (Environment)lstEnvironments.SelectedItem;
            selectedIndex = lstEnvironments.SelectedIndex;

            gridEnvironmentDetails.DataContext = selectedEnvironment;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (selectedEnvironment == null) { return; }

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                IndoorPositioningClient.UpdateEnvironment(selectedEnvironment);

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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedEnvironment == null) { return; }
            MessageBoxResult result = MessageBox.Show("Are you sure to delete this?", "WARNING!", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No) { return; }

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                IndoorPositioningClient.DeleteEnvironment(selectedEnvironment);

                /* remove selected item */
                selectedEnvironment = null;
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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                /* Create a new environment */
                Environment environment = new Environment()
                {
                    Name = txtName.Text,
                    Width = int.Parse(txtWidth.Text),
                    Height = int.Parse(txtHeight.Text),
                    Timestamp = DateTime.Now,
                };
                IndoorPositioningClient.AddEnvironment(environment);

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

        private void txt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex _regex = new Regex("\\d"); //regex that matches disallowed text
            e.Handled = !_regex.IsMatch(e.Text);
        }
    }
}
