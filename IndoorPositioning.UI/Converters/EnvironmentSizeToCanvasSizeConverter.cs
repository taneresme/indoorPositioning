using System;
using System.Globalization;
using System.Windows.Data;

namespace IndoorPositioning.UI.Converters
{
    public class EnvironmentSizeToCanvasSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            string paramm = ((string)parameter).ToLower();

            int environmentHeight = (int)value[0];
            int environmentWidth = (int)value[1];
            double screenHeight = (double)value[2];
            double screenWidth = (double)value[3];

            /* we are using the formula below to calculate the size of the 
             * canvas according to the size of screen :
             * Where x = the width of screen
             * Where y = the height of screen
             * Where x1 = the width of environment
             * Where y1 = the height of environment
             * Where x2 = the width of canvas to be calculated
             * Where y2 = the height of canvas to be calculated
             * y2 = y
             * x2 = x1 * (y / y1)
             * if x2 is greater than x:
             * x2 = x
             * y2 = y1 * (x / x1)
             */
            double mappedHeight = screenHeight;
            double mappedWidth = environmentWidth * (screenHeight / environmentHeight);
            if (mappedWidth > screenWidth)
            {
                mappedHeight = environmentHeight * (screenWidth / environmentWidth);
                mappedWidth = screenWidth;
            }

            if ("height".Equals(paramm)) { return mappedHeight; }
            if ("width".Equals(paramm)) { return mappedWidth; }

            /* Return by default */
            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
