using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ProBaumkarte_UWP.Converter
{
    public class MarkerFontSizeConverter:IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double fontsize;
            double markersize = (double)value;
          
            fontsize = markersize*0.8;


            return fontsize;
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
