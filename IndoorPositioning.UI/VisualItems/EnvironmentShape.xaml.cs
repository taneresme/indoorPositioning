using IndoorPositioning.UI.Converters;
using IndoorPositioning.UI.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace IndoorPositioning.UI.VisualItems
{
    public delegate void SelectedReferencePointChangedEventHandler(int xaxis, int yaxis);
    public delegate void AllReferencePointsUnselectedEventHandler();

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

        /* This event will be fired when the selection of the reference point changed */
        public event SelectedReferencePointChangedEventHandler SelectedReferencePointChanged;
        public void OnSelectedReferencePointChanged()
        {
            if (SelectedReferencePointChanged != null)
            {
                SelectedReferencePointChanged(SelectedXaxis, SelectedYaxis);
            }
        }

        /* All of the reference point on the map were unselected */
        public event AllReferencePointsUnselectedEventHandler AllReferencePointsUnselected;
        public void OnAllReferencePointsUnselected()
        {
            if (AllReferencePointsUnselected != null)
            {
                AllReferencePointsUnselected();
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

        
        public Gateway[] Gateways
        {
            get { return (Gateway[])GetValue(GatewaysProperty); }
            set { SetValue(GatewaysProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Gateways.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GatewaysProperty =
            DependencyProperty.Register("Gateways", typeof(Gateway[]), typeof(EnvironmentShape), new PropertyMetadata(null));
        

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
        /* Is there any selected point on the map */
        public bool IsThereAnySelectedPoint { get; set; }

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

        private BeaconShape beacon = null;
        public void MoveBeacon(int xaxis, int yaxis)
        {
            if (beacon == null)
            {
                beacon = new BeaconShape();
                canvas.Children.Add(beacon);
            }

            beacon.Xaxis = xaxis;
            beacon.Yaxis = yaxis;
        }

        private void OrganizeScreen()
        {
            /* Check the eligibility of the parameters */
            if (DistanceBetweenReferencePoints <= 0) return;
            if (EnvironmentHeight <= 0) return;
            if (EnvironmentWidth <= 0) return;

            /* Remove all of the reference point items from Canvas */
            /* PERFORMANCE NOTE:
             * The items in the canvas have the subscription to the event of ReferencePointSelectionStatusChanged.
             * But we are removing all without unsubscribing them from the event.
             * It will cause the memory leak.
             * */
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
                    shape.ReferencePointSelectionStatusChanged += Shape_ReferencePointSelectionStatusChanged;
                    canvas.Children.Add(shape);
                }
            }

            /* Add gateways to the screen */
            if (Gateways != null)
            {
                foreach (var gateway in Gateways)
                {
                    GatewayShape shape = new GatewayShape();

                    /* We are setting the y-axis value
                     * If the Yaxis value is O, the distance from left will be 0
                     * If the Yaxis value is N, the distance from left will be the width of the canvas 
                     * If the Yaxis value is N/2, the distance from left will be the half of the width of the canvas
                     * The converter class does the action above */
                    Canvas.SetLeft(shape,
                        (double)new GatewayPositionToCanvasCoordinateConverter()
                        .Convert(
                            new object[] {
                                gateway.Yaxis,
                                canvas.ActualHeight,
                                canvas.ActualWidth
                            }, null, "width", null));

                    /* We are setting the x-axis value
                     * If the Xaxis value is O, the distance from top will be 0
                     * If the Xaxis value is N, the distance from top will be the height of the canvas 
                     * If the Xaxis value is N/2, the distance from top will be the half of the height of the canvas
                     * The converter class does the action above */
                    Canvas.SetTop(shape,
                        (double)new GatewayPositionToCanvasCoordinateConverter()
                        .Convert(
                            new object[] {
                                gateway.Xaxis,
                                canvas.ActualHeight,
                                canvas.ActualWidth
                            }, null, "height", null));
                    
                    canvas.Children.Add(shape);
                }
            }
        }

        private void Shape_ReferencePointSelectionStatusChanged(bool status, int x, int y)
        {
            /* This event is only fired when the reference point is selected
             * or unselected by clicking on it */
            IsThereAnySelectedPoint = status;
            /* If the reference point is selected then change the points */
            if (status)
            {
                SelectedXaxis = x;
                SelectedYaxis = y;

                /* Notify the subscriber of the event */
                OnSelectedReferencePointChanged();
            }
            else
            {
                /* Notify the subscriber of the event */
                OnAllReferencePointsUnselected();
            }
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            OrganizeScreen();
        }
    }
}
