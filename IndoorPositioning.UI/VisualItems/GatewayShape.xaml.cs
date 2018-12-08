using System.Windows.Controls;

namespace IndoorPositioning.UI.VisualItems
{
    /// <summary>
    /// Interaction logic for GatewayShape.xaml
    /// </summary>
    public partial class GatewayShape : UserControl
    {
        public static int SIZE = 16;

        public GatewayShape()
        {
            InitializeComponent();

            Width = SIZE;
            Height = SIZE;
        }
    }
}
