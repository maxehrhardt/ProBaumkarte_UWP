using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ProBaumkarte_UWP.Converter
{
    public class MapModeToFalseConverter:IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {

            if (value.ToString() == parameter.ToString())
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
            {
                return null;
            }
            else
            {
                return false;
            }
        }
    }
}
