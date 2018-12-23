using IndoorPositioning.UI.Client;
using IndoorPositioning.UI.KNN;
using IndoorPositioning.UI.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Environment = IndoorPositioning.UI.Model.Environment;
using CenterSpace.NMath.Core;
using System.Diagnostics;
using IndoorPositioning.UI.Converters;

namespace IndoorPositioning.UI.Screens
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class MapScreen : UserControl, INotifyPropertyChanged, IDisposable
    {
        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    PositioningActivated = false;
                    Initialized -= MapScreen_Initialized;
                    Loaded -= MapScreen_Loaded;

                    StopPositioning();
                }

                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support

        /* Used to figure out whether the component loaded or not */
        private bool initialized = false;
        private Thread positioningThread;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        /* Stores selected index of the environment combobox */
        private static int selectedEnvironmentIndex = -1;
        public int SelectedEnvironmentIndex
        {
            get { return selectedEnvironmentIndex; }
            set
            {
                selectedEnvironmentIndex = value;
                OnPropertyChanged("SelectedEnvironmentIndex");
            }
        }

        /* Stores selected index of the algorithm combobox */
        private static int selectedAlgorithmIndex = -1;
        public int SelectedAlgorithmIndex
        {
            get { return selectedAlgorithmIndex; }
            set
            {
                selectedAlgorithmIndex = value;
                OnPropertyChanged("SelectedAlgorithmIndex");
            }
        }

        /* Stores selected index of the beacon combobox */
        private static int selectedBeaconIndex = -1;
        public int SelectedBeaconIndex
        {
            get { return selectedBeaconIndex; }
            set
            {
                selectedBeaconIndex = value;
                OnPropertyChanged("SelectedBeaconIndex");
            }
        }

        /* Stores selected index of the environment combobox */
        private bool positioningActivated = false;
        public bool PositioningActivated
        {
            get { return positioningActivated; }
            set
            {
                /* Check the inputs */
                if (value && (selectedBeaconIndex == -1 ||
                    selectedEnvironmentIndex == -1 ||
                    selectedAlgorithmIndex == -1))
                {
                    MessageBox.Show("Please do the selections!");
                    return;
                }
                positioningActivated = value;
                OnPropertyChanged("PositioningActivated");

                if (positioningActivated)
                {
                    StartPositioning();
                }
                else
                {
                    StopPositioning();
                }
            }
        }

        /* Indicates whether the loading screen is appearing or not */
        public bool ShowLoadingScreen
        {
            set
            {
                grdPleaseWait.Visibility = value
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        public MapScreen()
        {
            Initialized += MapScreen_Initialized;
            Loaded += MapScreen_Loaded;

            InitializeComponent();

            DataContext = this;

            /* Set gateways of the environment */
            try
            {
                List<Gateway> gateways = IndoorPositioningClient.GetGateways();
                environmentShape.Gateways = gateways.ToArray();
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void MapScreen_Loaded(object sender, RoutedEventArgs e)
        {
            if (!initialized) return;
            Load();
        }

        private void MapScreen_Initialized(object sender, EventArgs e)
        {
            initialized = true;
        }

        private void StopPositioning()
        {
            try
            {
                IndoorPositioningClient.SetModeAsIdle();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            try { positioningThread.Abort(); }
            catch { }
            try { positioningThread = null; }
            catch { }
        }

        /* Load the list of environments from Server */
        private void Load()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                cbEnvironments.ItemsSource = IndoorPositioningClient.GetEnvironments();
                cbBeacons.ItemsSource = IndoorPositioningClient.GetBeacons();
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                MessageBox.Show(ex.ToString());
            }
        }

        private void StartPositioning()
        {
            try
            {
                /* Change server mode into positioning */
                Beacon beacon = (Beacon)cbBeacons.SelectedItem;
                IndoorPositioningClient.SetModeAsPositioning(beacon.BeaconId);

                environment = (Environment)cbEnvironments.SelectedItem;

                positioningThread = new Thread(Positioning)
                {
                    IsBackground = true
                };
                positioningThread.Start();

                ShowLoadingScreen = true;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private Environment environment;
        private void Positioning()
        {
            try
            {
                /* Load fingerprinting data from the server */
                List<AdjustedFingerprinting> fingerprintings = IndoorPositioningClient.GetFingerprintings(environment.EnvironmentId);
                /* Disappear the loading splash screen */
                Application.Current.Dispatcher.Invoke(() => ShowLoadingScreen = false);

                while (PositioningActivated)
                {
                    try
                    {
                        Coordinate coordinate = new Coordinate();
                        /* Run KNNClassifier */
                        if (SelectedAlgorithmIndex == 0)
                        {
                            coordinate = Positioning_Knn(fingerprintings);
                        }
                        else if (SelectedAlgorithmIndex == 1)
                        {
                            coordinate = Positioning_KnnProximity(fingerprintings);
                        }
                        else if (SelectedAlgorithmIndex == 2)
                        {
                            coordinate = Positioning_Proximity();
                        }
                        else
                        {
                            throw new Exception("Invalid algorithm");
                        }

                        /* Success process, clear error message if any */
                        Application.Current.Dispatcher.Invoke(() => txtAlert.Text = "");

                        /* Add the beacon localized onto the screen */
                        Application.Current.Dispatcher.Invoke(() => environmentShape.MoveBeacon(coordinate.Xaxis, coordinate.Yaxis));
                    }
                    catch (ThreadAbortException) { break; }
                    catch (Exception ex)
                    {
                        Application.Current.Dispatcher.Invoke(() => txtAlert.Text = ex.Message);
                    }
                    finally
                    {
                        /* Every second, query the server for the RSSI values read by the gateways */
                        Thread.Sleep(2000);
                    }
                }
            }
            catch (ThreadAbortException) { /* do nothing */ }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => MessageBox.Show(ex.ToString()));
            }
        }

        private double GetGatewayXaxis(Gateway gateway)
        {
            /* We are setting the y-axis value
             * If the Yaxis value is O, the distance from left will be 0
             * If the Yaxis value is N, the distance from left will be the width of the canvas 
             * If the Yaxis value is N/2, the distance from left will be the half of the width of the canvas
             * The converter class does the action above */
            return (double)new GatewayPositionToCanvasCoordinateConverter()
                .Convert(
                    new object[] {
                        gateway.Yaxis,
                        (double)environment.Height,
                        (double)environment.Width
                    }, null, "width", null);
        }

        private double GetGatewayYaxis(Gateway gateway)
        {
            /* We are setting the x-axis value
             * If the Xaxis value is O, the distance from top will be 0
             * If the Xaxis value is N, the distance from top will be the height of the canvas 
             * If the Xaxis value is N/2, the distance from top will be the half of the height of the canvas
             * The converter class does the action above */
            return (double)new GatewayPositionToCanvasCoordinateConverter()
                .Convert(
                    new object[] {
                        gateway.Xaxis,
                        (double)environment.Height,
                        (double)environment.Width
                    }, null, "height", null);
        }

        private Coordinate Positioning_Knn(List<AdjustedFingerprinting> fingerprintings)
        {
            /* run the knn classifier on the data */
            KnnClassifier classifier = new KnnClassifier();

            /* After fetching and processing the fingerprinting data, I am able to get the class count.
             * Basically, each of the reference point is a class to be classified to. */
            int numClasses = IndoorPositioningClient.GetPoints(environment.EnvironmentId).Count;
            int gatewayCount = fingerprintings[0].RssiValueAndGateway.Count;

            /* get the Rssi values of the beacon in question from the server */
            RssiValue[] rssiValues = IndoorPositioningClient.GetRssi(gatewayCount);

            /* we will use also gateway count on the area as K constant */
            Coordinate coordinate = classifier.Classify(rssiValues, fingerprintings, numClasses, 3);
            return coordinate;
        }

        private Coordinate Positioning_KnnProximity(List<AdjustedFingerprinting> fingerprintings)
        {
            int k = 3;
            /* run the knn classifier on the data */
            KnnClassifier classifier = new KnnClassifier();

            /* After fetching and processing the fingerprinting data, I am able to get the class count.
             * Basically, each of the reference point is a class to be classified to. */
            int numClasses = IndoorPositioningClient.GetPoints(environment.EnvironmentId).Count;
            int gatewayCount = fingerprintings[0].RssiValueAndGateway.Count;

            /* get the Rssi values of the beacon in question from the server */
            RssiValue[] rssiValues = IndoorPositioningClient.GetRssi(gatewayCount);

            /* we will use also gateway count on the area as K constant */
            CoordinateAndDistance[] cooDist = classifier.GetNearestNeighbors(rssiValues, fingerprintings, numClasses, k);
            Coordinate coordinateFirstAndSecondLine = new Coordinate();
            Coordinate coordinateFirstAndThirdLine = new Coordinate();
            Coordinate coordinateSecondAndThirdLine = new Coordinate();

            /* Create a vector from the coordinates */
            /* Measure the first point of crossing the first and second line*/
            DoubleMatrix matrixFirstAndSecondLine = new DoubleMatrix(new double[,]
            {
                {
                    cooDist[0].coordinate.Xaxis - cooDist[1].coordinate.Xaxis,
                    cooDist[0].coordinate.Yaxis - cooDist[1].coordinate.Yaxis
                },
                {
                    cooDist[0].coordinate.Xaxis - cooDist[2].coordinate.Xaxis,
                    cooDist[0].coordinate.Yaxis - cooDist[2].coordinate.Yaxis
                }
            });

            /* If the determinant value of the matrix is 0, it means this matrix cannot be inverted.
             * We can have the exact point values with one of the reference points. */
            if (NMathFunctions.Determinant(matrixFirstAndSecondLine) == 0)
            {
                return classifier.Vote(cooDist, fingerprintings, numClasses, k);
            }

            //matrixFirstAndSecondLine = matrixFirstAndSecondLine.Transform((p) => p * 2);
            DoubleMatrix inversedMatrixFirstAndSecondLine = NMathFunctions.Inverse(matrixFirstAndSecondLine);
            DoubleVector vectorFirstAndSecondLine = new DoubleVector(new double[]
            {
                cooDist[0].coordinate.GetNormPow() - cooDist[1].coordinate.GetNormPow() - cooDist[0].GetDistPow() + cooDist[1].GetDistPow(),
                cooDist[0].coordinate.GetNormPow() - cooDist[2].coordinate.GetNormPow() - cooDist[0].GetDistPow() + cooDist[2].GetDistPow()
            });
            vectorFirstAndSecondLine = vectorFirstAndSecondLine.Transform((p) => p / 2);

            DoubleVector coordinateVectorFirstAndSecondLine = NMathFunctions.Product(inversedMatrixFirstAndSecondLine,
                vectorFirstAndSecondLine);
            coordinateFirstAndSecondLine.Xaxis = (int)coordinateVectorFirstAndSecondLine[0];
            coordinateFirstAndSecondLine.Yaxis = (int)coordinateVectorFirstAndSecondLine[1];

            /* Measure the first point of crossing the first and third line*/
            DoubleMatrix matrixFirstAndThirdLine = new DoubleMatrix(new double[,]
            {
                {
                    cooDist[0].coordinate.Xaxis - cooDist[1].coordinate.Xaxis,
                    cooDist[0].coordinate.Yaxis - cooDist[1].coordinate.Yaxis
                },
                {
                    cooDist[1].coordinate.Xaxis - cooDist[2].coordinate.Xaxis,
                    cooDist[1].coordinate.Yaxis - cooDist[2].coordinate.Yaxis
                }
            });

            /* If the determinant value of the matrix is 0, it means this matrix cannot be inverted.
             * We can have the exact point values with one of the reference points. */
            if (NMathFunctions.Determinant(matrixFirstAndThirdLine) == 0)
            {
                return classifier.Vote(cooDist, fingerprintings, numClasses, k);
            }

            //matrixFirstAndThirdLine = matrixFirstAndThirdLine.Transform((p) => p * 2);
            DoubleMatrix inversedMatrixFirstAndThirdLine = NMathFunctions.Inverse(matrixFirstAndThirdLine);
            DoubleVector vectorFirstAndThirdLine = new DoubleVector(new double[]
            {
                cooDist[0].coordinate.GetNormPow() - cooDist[1].coordinate.GetNormPow() - cooDist[0].GetDistPow() + cooDist[1].GetDistPow(),
                cooDist[1].coordinate.GetNormPow() - cooDist[2].coordinate.GetNormPow() - cooDist[1].GetDistPow() + cooDist[2].GetDistPow()
            });
            vectorFirstAndThirdLine = vectorFirstAndThirdLine.Transform((p) => p / 2);

            DoubleVector coordinateVectorFirstAndThirdLine = NMathFunctions.Product(inversedMatrixFirstAndThirdLine,
                vectorFirstAndThirdLine);
            coordinateFirstAndThirdLine.Xaxis = (int)coordinateVectorFirstAndThirdLine[0];
            coordinateFirstAndThirdLine.Yaxis = (int)coordinateVectorFirstAndThirdLine[1];

            /* Measure the first point of crossing the first and third line*/
            DoubleMatrix matrixSecondAndThirdLine = new DoubleMatrix(new double[,]
            {
                {
                    cooDist[0].coordinate.Xaxis - cooDist[2].coordinate.Xaxis,
                    cooDist[0].coordinate.Yaxis - cooDist[2].coordinate.Yaxis
                },
                {
                    cooDist[1].coordinate.Xaxis - cooDist[2].coordinate.Xaxis,
                    cooDist[1].coordinate.Yaxis - cooDist[2].coordinate.Yaxis
                }
            });

            /* If the determinant value of the matrix is 0, it means this matrix cannot be inverted.
             * We can have the exact point values with one of the reference points. */
            if (NMathFunctions.Determinant(matrixSecondAndThirdLine) == 0)
            {
                return classifier.Vote(cooDist, fingerprintings, numClasses, k);
            }

            //matrixSecondAndThirdLine = matrixSecondAndThirdLine.Transform((p) => p * 2);
            DoubleMatrix inversedMatrixSecondAndThirdLine = NMathFunctions.Inverse(matrixSecondAndThirdLine);
            DoubleVector vectorSecondAndThirdLine = new DoubleVector(new double[]
            {
                cooDist[0].coordinate.GetNormPow() - cooDist[2].coordinate.GetNormPow() - cooDist[0].GetDistPow() + cooDist[2].GetDistPow(),
                cooDist[1].coordinate.GetNormPow() - cooDist[2].coordinate.GetNormPow() - cooDist[1].GetDistPow() + cooDist[2].GetDistPow()
            });
            vectorSecondAndThirdLine = vectorSecondAndThirdLine.Transform((p) => p / 2);

            DoubleVector coordinateVectorSecondAndThirdLine = NMathFunctions.Product(inversedMatrixSecondAndThirdLine,
                vectorSecondAndThirdLine);
            coordinateSecondAndThirdLine.Xaxis = (int)coordinateVectorSecondAndThirdLine[0];
            coordinateSecondAndThirdLine.Yaxis = (int)coordinateVectorSecondAndThirdLine[1];

            //DoubleMatrix matrixFirstAndThirdLine = new DoubleMatrix(new double[,]
            //{
            //    {
            //        cooDist[0].coordinate.Xaxis - cooDist[1].coordinate.Xaxis,
            //        cooDist[0].coordinate.Yaxis - cooDist[1].coordinate.Yaxis
            //    },
            //    {
            //        cooDist[0].coordinate.Xaxis - cooDist[2].coordinate.Xaxis,
            //        cooDist[0].coordinate.Yaxis - cooDist[2].coordinate.Yaxis
            //    },
            //    {
            //        cooDist[1].coordinate.Xaxis - cooDist[2].coordinate.Xaxis,
            //        cooDist[1].coordinate.Yaxis - cooDist[2].coordinate.Yaxis
            //    }
            //});
            //DoubleMatrix inversedMatrix = NMathFunctions.Inverse(matrix);
            //DoubleVector vector = new DoubleVector(new double[]
            //{
            //    cooDist[0].coordinate.GetNormPow() - cooDist[1].coordinate.GetNormPow() - cooDist[0].GetDistPow() + cooDist[1].GetDistPow(),
            //    cooDist[0].coordinate.GetNormPow() - cooDist[2].coordinate.GetNormPow() - cooDist[0].GetDistPow() + cooDist[2].GetDistPow(),
            //    cooDist[1].coordinate.GetNormPow() - cooDist[2].coordinate.GetNormPow() - cooDist[1].GetDistPow() + cooDist[2].GetDistPow()
            //});
            //DoubleVector coordinateVector = NMathFunctions.Product(inversedMatrix, vector);
            //coordinate.Xaxis = (int)coordinateVector[0];
            //coordinate.Xaxis = (int)coordinateVector[1];

            //Matrix linesMatrix = new Matrix(
            //    cooDist[0].coordinate.Xaxis - cooDist[1].coordinate.Xaxis,
            //    cooDist[0].coordinate.Yaxis - cooDist[1].coordinate.Yaxis,
            //    cooDist[0].coordinate.Xaxis - cooDist[2].coordinate.Xaxis,
            //    cooDist[0].coordinate.Yaxis - cooDist[2].coordinate.Yaxis,
            //    cooDist[1].coordinate.Xaxis - cooDist[2].coordinate.Xaxis,
            //    cooDist[1].coordinate.Yaxis - cooDist[2].coordinate.Yaxis);

            ////Vector distances = new Vector(
            ////    coordinateAndDistances[0].coordinate.Xaxis - coordinateAndDistances[1].coordinate.Xaxis,);

            int xaxis =
                (coordinateFirstAndSecondLine.Xaxis +
                coordinateFirstAndThirdLine.Xaxis +
                coordinateSecondAndThirdLine.Xaxis) / 3;
            int yaxis =
                (coordinateFirstAndSecondLine.Yaxis +
                coordinateFirstAndThirdLine.Yaxis +
                coordinateSecondAndThirdLine.Yaxis) / 3;

            Debug.WriteLine("Xaxis:" + xaxis);
            Debug.WriteLine("Yaxis:" + yaxis);

            return new Coordinate()
            {
                Xaxis = xaxis,
                Yaxis = yaxis
            };
        }

        private Coordinate Positioning_Proximity()
        {
            List<Gateway> gateways = IndoorPositioningClient.GetGateways();

            /* get the Rssi values of the beacon in question from the server */
            RssiValue[] rssiValues = IndoorPositioningClient.GetRssi(gateways.Count);

            CoordinateAndDistance[] cooDist = new CoordinateAndDistance[gateways.Count];
            for (int i = 0; i < gateways.Count; i++)
            {
                cooDist[i] = new CoordinateAndDistance()
                {
                    coordinate = new Coordinate()
                    {
                        Xaxis = (int)GetGatewayXaxis(gateways[i]),
                        Yaxis = (int)GetGatewayYaxis(gateways[i]),
                    },
                    dist = rssiValues[i].Rssi
                };
            }

            Coordinate coordinateFirstAndSecondLine = new Coordinate();
            Coordinate coordinateFirstAndThirdLine = new Coordinate();
            Coordinate coordinateSecondAndThirdLine = new Coordinate();

            /* Create a vector from the coordinates */
            /* Measure the first point of crossing the first and second line*/
            DoubleMatrix matrixFirstAndSecondLine = new DoubleMatrix(new double[,]
            {
                {
                    cooDist[0].coordinate.Xaxis - cooDist[1].coordinate.Xaxis,
                    cooDist[0].coordinate.Yaxis - cooDist[1].coordinate.Yaxis
                },
                {
                    cooDist[0].coordinate.Xaxis - cooDist[2].coordinate.Xaxis,
                    cooDist[0].coordinate.Yaxis - cooDist[2].coordinate.Yaxis
                }
            });

            /* If the determinant value of the matrix is 0, it means this matrix cannot be inverted.
             * We can have the exact point values with one of the reference points. */
            if (NMathFunctions.Determinant(matrixFirstAndSecondLine) == 0)
            {
                return cooDist[0].coordinate;
            }

            //matrixFirstAndSecondLine = matrixFirstAndSecondLine.Transform((p) => p * 2);
            DoubleMatrix inversedMatrixFirstAndSecondLine = NMathFunctions.Inverse(matrixFirstAndSecondLine);
            DoubleVector vectorFirstAndSecondLine = new DoubleVector(new double[]
            {
                cooDist[0].coordinate.GetNormPow() - cooDist[1].coordinate.GetNormPow() - cooDist[0].GetDistPow() + cooDist[1].GetDistPow(),
                cooDist[0].coordinate.GetNormPow() - cooDist[2].coordinate.GetNormPow() - cooDist[0].GetDistPow() + cooDist[2].GetDistPow()
            });
            vectorFirstAndSecondLine = vectorFirstAndSecondLine.Transform((p) => p / 2);

            DoubleVector coordinateVectorFirstAndSecondLine = NMathFunctions.Product(inversedMatrixFirstAndSecondLine,
                vectorFirstAndSecondLine);
            coordinateFirstAndSecondLine.Xaxis = (int)coordinateVectorFirstAndSecondLine[0];
            coordinateFirstAndSecondLine.Yaxis = (int)coordinateVectorFirstAndSecondLine[1];

            /* Measure the first point of crossing the first and third line*/
            DoubleMatrix matrixFirstAndThirdLine = new DoubleMatrix(new double[,]
            {
                {
                    cooDist[0].coordinate.Xaxis - cooDist[1].coordinate.Xaxis,
                    cooDist[0].coordinate.Yaxis - cooDist[1].coordinate.Yaxis
                },
                {
                    cooDist[1].coordinate.Xaxis - cooDist[2].coordinate.Xaxis,
                    cooDist[1].coordinate.Yaxis - cooDist[2].coordinate.Yaxis
                }
            });

            /* If the determinant value of the matrix is 0, it means this matrix cannot be inverted.
             * We can have the exact point values with one of the reference points. */
            if (NMathFunctions.Determinant(matrixFirstAndThirdLine) == 0)
            {
                return cooDist[0].coordinate;
            }

            //matrixFirstAndThirdLine = matrixFirstAndThirdLine.Transform((p) => p * 2);
            DoubleMatrix inversedMatrixFirstAndThirdLine = NMathFunctions.Inverse(matrixFirstAndThirdLine);
            DoubleVector vectorFirstAndThirdLine = new DoubleVector(new double[]
            {
                cooDist[0].coordinate.GetNormPow() - cooDist[1].coordinate.GetNormPow() - cooDist[0].GetDistPow() + cooDist[1].GetDistPow(),
                cooDist[1].coordinate.GetNormPow() - cooDist[2].coordinate.GetNormPow() - cooDist[1].GetDistPow() + cooDist[2].GetDistPow()
            });
            vectorFirstAndThirdLine = vectorFirstAndThirdLine.Transform((p) => p / 2);

            DoubleVector coordinateVectorFirstAndThirdLine = NMathFunctions.Product(inversedMatrixFirstAndThirdLine,
                vectorFirstAndThirdLine);
            coordinateFirstAndThirdLine.Xaxis = (int)coordinateVectorFirstAndThirdLine[0];
            coordinateFirstAndThirdLine.Yaxis = (int)coordinateVectorFirstAndThirdLine[1];

            /* Measure the first point of crossing the first and third line*/
            DoubleMatrix matrixSecondAndThirdLine = new DoubleMatrix(new double[,]
            {
                {
                    cooDist[0].coordinate.Xaxis - cooDist[2].coordinate.Xaxis,
                    cooDist[0].coordinate.Yaxis - cooDist[2].coordinate.Yaxis
                },
                {
                    cooDist[1].coordinate.Xaxis - cooDist[2].coordinate.Xaxis,
                    cooDist[1].coordinate.Yaxis - cooDist[2].coordinate.Yaxis
                }
            });

            /* If the determinant value of the matrix is 0, it means this matrix cannot be inverted.
             * We can have the exact point values with one of the reference points. */
            if (NMathFunctions.Determinant(matrixSecondAndThirdLine) == 0)
            {
                return cooDist[0].coordinate;
            }

            //matrixSecondAndThirdLine = matrixSecondAndThirdLine.Transform((p) => p * 2);
            DoubleMatrix inversedMatrixSecondAndThirdLine = NMathFunctions.Inverse(matrixSecondAndThirdLine);
            DoubleVector vectorSecondAndThirdLine = new DoubleVector(new double[]
            {
                cooDist[0].coordinate.GetNormPow() - cooDist[2].coordinate.GetNormPow() - cooDist[0].GetDistPow() + cooDist[2].GetDistPow(),
                cooDist[1].coordinate.GetNormPow() - cooDist[2].coordinate.GetNormPow() - cooDist[1].GetDistPow() + cooDist[2].GetDistPow()
            });
            vectorSecondAndThirdLine = vectorSecondAndThirdLine.Transform((p) => p / 2);

            DoubleVector coordinateVectorSecondAndThirdLine = NMathFunctions.Product(inversedMatrixSecondAndThirdLine,
                vectorSecondAndThirdLine);
            coordinateSecondAndThirdLine.Xaxis = (int)coordinateVectorSecondAndThirdLine[0];
            coordinateSecondAndThirdLine.Yaxis = (int)coordinateVectorSecondAndThirdLine[1];

            int xaxis =
                (coordinateFirstAndSecondLine.Xaxis +
                coordinateFirstAndThirdLine.Xaxis +
                coordinateSecondAndThirdLine.Xaxis) / 3;
            int yaxis =
                (coordinateFirstAndSecondLine.Yaxis +
                coordinateFirstAndThirdLine.Yaxis +
                coordinateSecondAndThirdLine.Yaxis) / 3;

            Debug.WriteLine("Xaxis:" + xaxis);
            Debug.WriteLine("Yaxis:" + yaxis);

            return new Coordinate()
            {
                Xaxis = xaxis,
                Yaxis = yaxis
            };
        }
    }
}
