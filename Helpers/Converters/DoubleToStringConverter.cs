using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using System.Diagnostics;

namespace humanlab.Helpers.Converters
{
    class DoubleToStringConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                Debug.WriteLine("j***********************************************");
                return value.ToString();


            }
            else
                return "";
        }
    

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                try
                {
                    Debug.WriteLine("jikoijoiiiiiiiiiiiiiiiii");
                    Double v = (Double)value;
                    Debug.WriteLine("type: " + v.GetType());
                    return v;
                }
                catch
                {
                    return null;

                }
            }
            else
                return null;

        }
    }
}
