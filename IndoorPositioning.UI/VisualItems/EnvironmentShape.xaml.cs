using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace IndoorPositioning.UI.VisualItems
{
    /// <summary>
    /// Interaction logic for EnvironmentShape.xaml
    /// </summary>
    public partial class EnvironmentShape : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #region Dependency Properties

        /* Width value of the environment */
        public int EnvironmentWidth
        {
            get { return (int)GetValue(EnvironmentWidthProperty); }
            set { SetValue(EnvironmentWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnvironmentWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnvironmentWidthProperty =
            DependencyProperty.Register("EnvironmentWidth", typeof(int), typeof(EnvironmentShape),
                new PropertyMetadata(OnPropertyChangedCallback));



        /* Height value of the environment */
        public int EnvironmentHeight
        {
            get { return (int)GetValue(EnvironmentHeightProperty); }
            set { SetValue(EnvironmentHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnvironmentHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnvironmentHeightProperty =
            DependencyProperty.Register("EnvironmentHeight", typeof(int), typeof(EnvironmentShape),
                new PropertyMetadata(OnPropertyChangedCallback));



        /* The distance between the findgerprinting points */
        public int DistanceBetweenReferencePoints
        {
            get { return (int)GetValue(DistanceBetweenReferencePointsProperty); }
            set { SetValue(DistanceBetweenReferencePointsProperty, value); }
        }
        // Using a DependencyProperty as the backing store for DistanceBetweenReferencePoints.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DistanceBetweenReferencePointsProperty =
            DependencyProperty.Register("DistanceBetweenReferencePoints", typeof(int), typeof(EnvironmentShape),
                new PropertyMetadata(OnPropertyChangedCallback));


        /* Notify the other internal properties. */
        private static void OnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EnvironmentShape shape = (EnvironmentShape)d;
            shape.OnPropertyChanged(e.Property.Name);
            shape.OrganizeScreen();
        }

        #endregion Dependency Properties

        /* X-axis of the selected reference point */
        public int SelectedXaxis { get; set; }
        /* Y-axis of the selected reference point */
        public int SelectedYaxis { get; set; }

        /* The reference points on the screen are selectable? */
        private bool selectable = true;
        public bool Selectable {
            get { return selectable; }
            set
            {
                selectable = value;
                gridToDisableSelectability.Visibility = selectable 
                    ? Visibility.Collapsed 
                    : Visibility.Visible;
            }
        }

        public EnvironmentShape()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void OrganizeScreen()
        {
            /* Check the eligibility of the parameters */
            if (DistanceBetweenReferencePoints <= 0) return;
            if (EnvironmentHeight <= 0) return;
            if (EnvironmentWidth <= 0) return;

            /* Remove all reference point items from Canvas */
            canvas.Children.Clear();

            for (int i = DistanceBetweenReferencePoints; i < (int)canvas.ActualWidth - ReferencePointShape.SIZE;
                i += DistanceBetweenReferencePoints + ReferencePointShape.SIZE)
            {
                for (int j = DistanceBetweenReferencePoints; j < (int)canvas.ActualHeight - ReferencePointShape.SIZE;
                    j += DistanceBetweenReferencePoints + ReferencePointShape.SIZE)
                {
                    ReferencePointShape shape = new ReferencePointShape()
                    {
                        Xaxis = i,
                        Yaxis = j
                    };
                    canvas.Children.Add(shape);
                }
            }
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            OrganizeScreen();
        }
    }
}
