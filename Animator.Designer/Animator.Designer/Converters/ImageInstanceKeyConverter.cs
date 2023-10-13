using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Animator.Designer.Converters
{
    public class ImageInstanceKeyConverter : IValueConverter
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

                Image imageInstance = new Image();
                imageInstance.Source = image;

                return imageInstance;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
