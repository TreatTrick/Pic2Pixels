using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Pic2PixelStylet.Converters
{
    internal class WidthHeightToSizeConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            if (values.Length != 2)
            {
                throw new ArgumentException("Two values expected");
            }
            if (
                !double.TryParse(values[0].ToString(), out double width)
                || !double.TryParse(values[1].ToString(), out double height)
            )
            {
                throw new ArgumentException("Both values should be doubles");
            }
            return new System.Windows.Size(width, height);
        }

        public object[] ConvertBack(
            object value,
            Type[] targetTypes,
            object parameter,
            CultureInfo culture
        )
        {
            throw new NotImplementedException();
        }
    }
}
