using System.ComponentModel;
using System.Windows.Controls;

namespace IndoorPositioning.UI.VisualItems
{

    public delegate void ReferencePointSelectedEventHandler(int x, int y);
    /// <summary>
    /// Interaction logic for ReferencePointShape.xaml
    /// </summary>
    public partial class ReferencePointShape : UserControl, INotifyPropertyChanged
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

        /* Event to be fired when one of the properties changed */
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        /* Stores whether this point selected or not */
        private bool isSelected = false;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
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

            DataContext = this;
        }

        private void SetSizes(int size)
        {
            Width = size;
            Height = size;
        }

        /* I want to bo able to uncheck a radio button after clicking on it again.
         * The implementation below is for that */
        private bool justChecked = false;
        private void radiobutton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!justChecked && IsSelected) { IsSelected = false; }
            justChecked = false;
        }

        private void radiobutton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            justChecked = true;
        }
    }
}
