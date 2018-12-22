using System.Windows.Controls;

namespace IndoorPositioning.UI.VisualItems
{
    /// <summary>
    /// Interaction logic for BeaconShape.xaml
    /// </summary>
    public partial class BeaconShape : UserControl
    {
        public static int SIZE = 16;

        /* X-axis of this element in canvas (environment) */
        private int xaxis;
        public int Xaxis
        {
            get { return xaxis; }
            set
            {
                xaxis = value;
                Canvas.SetLeft(this, (double)xaxis);
            }
        }

        /* Y-axis of this element in canvas (environment) */
        private int yaxis;
        public int Yaxis
        {
            get { return yaxis; }
            set
            {
                yaxis = value;
                Canvas.SetTop(this, (double)yaxis);
            }
        }

        public BeaconShape()
        {
            InitializeComponent();

            Width = SIZE;
            Height = SIZE;
        }
    }
}
