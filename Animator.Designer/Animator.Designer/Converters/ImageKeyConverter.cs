using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Animator.Designer.Converters
{
    public class ImageKeyConverter : IValueConverter
    {
        private string prefix = "pack://application:,,,/Animator.Designer;component/Resources/Images/Icons/";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string resourceName)
            {
                if (String.IsNullOrEmpty(resourceName))
                    return Binding.DoNothing;

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(prefix + resourceName);
                image.EndInit();

                return image;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
