using IndoorPositioning.UI.VisualItems;
using System;
using System.Globalization;
using System.Windows.Data;

namespace IndoorPositioning.UI.Converters
{
    public class GatewayPositionToCanvasCoordinateConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            string paramm = ((string)parameter).ToLower();

            string axis = (string)value[0];
            double canvasHeight = (double)value[1];
            double canvasWidth = (double)value[2];
            if ("0".Equals(axis)) return 0.0;
            /* Height of the gateway shape is SIZE. That's why we subtract SIZE. */
            if ("N".Equals(axis) && "height".Equals(paramm)) return canvasHeight - GatewayShape.SIZE;
            /* Width of the gateway shape is SIZE. That's why we subtract SIZE. */
            if ("N".Equals(axis) && "width".Equals(paramm)) return canvasWidth - GatewayShape.SIZE;
            if ("N/2".Equals(axis) && "height".Equals(paramm)) return canvasHeight / 2.0;
            if ("N/2".Equals(axis) && "width".Equals(paramm)) return canvasWidth / 2.0;

            /* Return by default */
            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
