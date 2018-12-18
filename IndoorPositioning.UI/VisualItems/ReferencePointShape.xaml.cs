using System.ComponentModel;
using System.Windows.Controls;

namespace IndoorPositioning.UI.VisualItems
{
    public delegate void ReferencePointSelectionStatusChangedEventHandler(bool status, int x, int y);

    /// <summary>
    /// Interaction logic for ReferencePointShape.xaml
    /// </summary>
    public partial class ReferencePointShape : UserControl, INotifyPropertyChanged
    {
        public static int SIZE = 16;

        /* The event to bve fired when the reference point is selected */
        public event ReferencePointSelectionStatusChangedEventHandler ReferencePointSelectionStatusChanged;
        public void OnReferencePointSelectionStatusChanged(bool status)
        {
            if (ReferencePointSelectionStatusChanged != null)
            {
                ReferencePointSelectionStatusChanged(status, Xaxis, Yaxis);
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
                /* ıf the current value of the property did not be changed,
                 * return with doing nothing. */
                if (isSelected == value) return;

                isSelected = value;

                OnPropertyChanged("IsSelected");

                /* If this reference point is selected them we are firing the event. 
                 * Because these reference points are radio buttons and when one of 
                 * the other reference points is selected, the other one that is selected 
                 * before will be changed into the unselected status.
                 * But we do not need to handle this kind of changes. We only need to handle 
                 * the changes by clicking on the same reference point. But we cannot get
                 * why the status of the reference point is changing here. That's why
                 * we are not handling the false status here. */
                if (isSelected)
                {
                    OnReferencePointSelectionStatusChanged(isSelected);
                }
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
            if (!justChecked && IsSelected)
            {
                IsSelected = false;

                /* Hiring the event because here we are changing the status of
                 the reference point by clicking on it. This case is the case that
                 we do not handle in the set method of IsSelected property. */
                OnReferencePointSelectionStatusChanged(false);
            }
            justChecked = false;
        }

        private void radiobutton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            justChecked = true;
        }
    }
}
