using IndoorPositioning.UI.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace IndoorPositioning.UI.KNN
{
    public class KnnClassifier
    {
        public Coordinate Vote(CoordinateAndDistance[] info, List<AdjustedFingerprinting> trainData, int numClasses, int k)
        {
            int[] votes = new int[numClasses];  // one cell per class
            for (int i = 0; i < k; ++i)  // just first k nearest
            {
                //int idx = info[i].idx;  // which item
                ////AdjustedFingerprinting classs = info[i].fingerprinting;  // class in last cell
                ////++votes[classs];
                //++votes[idx];
                info[i].coordinate.HitCount++;
            }

            Coordinate axis = info[0].coordinate;
            for (int i = 1; i < info.Length; i++)
            {
                if (axis.HitCount < info[i].coordinate.HitCount)
                {
                    axis = info[i].coordinate;
                }
            }
            return axis;
        }

        /* Euclidean Distance calculation */
        private double Distance(RssiValue[] unknown, AdjustedFingerprinting data)
        {
            double distance = 0.0;
            for (int i = 0; i < unknown.Length; ++i)
            {
                distance += (unknown[i].Rssi - data.RssiValueAndGateway[i].Rssi) *
                      (unknown[i].Rssi - data.RssiValueAndGateway[i].Rssi);
            }
            return Math.Sqrt(distance);
        }

        public Coordinate Classify(RssiValue[] unknown, List<AdjustedFingerprinting> trainData, int numClasses, int k)
        {
            // compute and store distances from unknown to all train data 
            int n = trainData.Count;  // number data items
            CoordinateAndDistance[] info = new CoordinateAndDistance[n];
            for (int i = 0; i < n; ++i)
            {
                CoordinateAndDistance curr = new CoordinateAndDistance();
                double dist = Distance(unknown, trainData[i]);
                curr.dist = dist;
                curr.coordinate = trainData[i].Coordinates;
                info[i] = curr;
            }

            Array.Sort(info);  // sort by distance
            Debug.WriteLine("\nNearest / Distance");
            Debug.WriteLine("==============================");
            for (int i = 0; i < k; ++i)
            {
                Coordinate classs = info[i].coordinate;
                string dist = info[i].dist.ToString("F3");
                Debug.WriteLine($"( {classs.Xaxis},{classs.Yaxis} ) / {dist}");
            }

            Coordinate result = Vote(info, trainData, numClasses, k);  // k nearest classes
            return result;
        }

        public CoordinateAndDistance[] GetNearestNeighbors(RssiValue[] unknown, List<AdjustedFingerprinting> trainData, int numClasses, int k)
        {
            // compute and store distances from unknown to all train data 
            int n = trainData.Count;  // number data items
            CoordinateAndDistance[] info = new CoordinateAndDistance[n];
            for (int i = 0; i < n; ++i)
            {
                CoordinateAndDistance curr = new CoordinateAndDistance();
                double dist = Distance(unknown, trainData[i]);
                curr.dist = dist;
                curr.coordinate = trainData[i].Coordinates;
                info[i] = curr;
            }

            CoordinateAndDistance[] coordinateAndDistances = new CoordinateAndDistance[k];
            Array.Sort(info);  // sort by distance
            Debug.WriteLine("\nNearest / Distance");
            Debug.WriteLine("==============================");
            for (int i = 0; i < k; ++i)
            {
                Coordinate classs = info[i].coordinate;
                string dist = info[i].dist.ToString("F3");
                Debug.WriteLine($"( {classs.Xaxis},{classs.Yaxis} ) / {dist}");

                coordinateAndDistances[i] = info[i];
            }

            return coordinateAndDistances;
        }
    }

    public class CoordinateAndDistance : IComparable<CoordinateAndDistance>
    {
        public double dist;  // distance from train item to unknown
        public Coordinate coordinate;

        // need to sort these to find k closest
        public int CompareTo(CoordinateAndDistance other)
        {
            if (this.dist < other.dist) return -1;
            else if (this.dist > other.dist) return +1;
            else return 0;
        }

        public double GetDistPow()
        {
            return Math.Pow(dist, 2);
        }
    }
}
