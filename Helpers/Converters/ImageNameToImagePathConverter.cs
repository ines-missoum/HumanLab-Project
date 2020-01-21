using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace humanlab.Helpers.Converters
{
    public class ImageNameToImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                BitmapImage image = new BitmapImage();
                image.UriSource = new Uri("ms-appx:///Assets/" + value.ToString());

                return image;
            }
                
            else
                return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                var split = value.ToString().Split('/');
                return split.Last();
            }    
            else
                return "";
        }
    }
}
