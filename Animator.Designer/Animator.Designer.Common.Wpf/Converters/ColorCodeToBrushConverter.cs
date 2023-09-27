using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Animator.Designer.Common.Wpf.Converters
{
    public class ColorCodeToBrushConverter : IValueConverter
    {
        private static Dictionary<string, Brush> brushCache = new();
        private static BrushConverter brushConverter = new BrushConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                if (brushCache.ContainsKey(str))
                    return brushCache[str];

                var brush = (Brush)brushConverter.ConvertFromString(str);
                brushCache[str] = brush;

                return brush;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
