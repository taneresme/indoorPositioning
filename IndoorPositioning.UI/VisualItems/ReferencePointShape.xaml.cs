using System.Windows.Controls;

namespace IndoorPositioning.UI.VisualItems
{

    public delegate void ReferencePointSelectedEventHandler(int x, int y);
    /// <summary>
    /// Interaction logic for ReferencePointShape.xaml
    /// </summary>
    public partial class ReferencePointShape : UserControl
    {
        public static int SIZE = 16;

        /* The event to bve fired when the reference point is selected */
        public event ReferencePointSelectedEventHandler ReferencePointSelected;
        public void OnReferencePointSelected()
        {
            if (ReferencePointSelected != null)
            {
                ReferencePointSelected(Xaxis, Yaxis);
            }
        }

        /* Returns if the reference point is selected. */
        public bool IsSelected
        {
            get
            {
                return radiobutton.IsChecked.HasValue
                  ? radiobutton.IsChecked.Value
                  : false;
            }
        }

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

        public ReferencePointShape()
        {
            InitializeComponent();

            SetSizes(SIZE);
        }

        private void SetSizes(int size)
        {
            Width = size;
            Height = size;
        }
    }
}
