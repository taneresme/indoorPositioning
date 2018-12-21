using System.Windows.Controls;

namespace IndoorPositioning.UI.VisualItems
{
    /// <summary>
    /// Interaction logic for BeaconShape.xaml
    /// </summary>
    public partial class BeaconShape : UserControl
    {
        public static int SIZE = 16;

        public BeaconShape()
        {
            InitializeComponent();

            Width = SIZE;
            Height = SIZE;
        }
    }
}
