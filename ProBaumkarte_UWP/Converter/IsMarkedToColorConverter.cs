using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ProBaumkarte_UWP.Converter
{
    public class IsMarkedToColorConverter:IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType, object parameter, string language)
        {

            if ((bool)value)
            {
                return new SolidColorBrush(Windows.UI.Colors.OrangeRed);
            }
            else
            {
                return new SolidColorBrush(Windows.UI.Colors.ForestGreen);
            }
            //// Retrieve the format string and use it to format the value.
            //string formatString = parameter as string;
            //if (!string.IsNullOrEmpty(formatString))
            //{
            //    return string.Format(
            //        new CultureInfo(language), formatString, value);
            //}
            //// If the format string is null or empty, simply call ToString()
            //// on the value.
            //return value.ToString();
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
            {
                return parameter;
            }
            else
            {
                return null;
            }
        }
    }
}
