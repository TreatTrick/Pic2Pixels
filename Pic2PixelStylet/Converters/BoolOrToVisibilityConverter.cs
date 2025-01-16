using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Pic2PixelStylet.Converters
{
    internal class BoolOrToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            bool isInverse = false;
            if (parameter is string s && s.ToLower() == "inverse")
            {
                isInverse = true;
            }
            else
            {
                throw new ArgumentException("Parameter must be of type string.");
            }

            foreach (var value in values)
            {
                if (value is bool b)
                {
                    if (b == true)
                        return true && !isInverse;
                }
                else
                {
                    throw new ArgumentException("All values must be of type bool.");
                }
            }
            return false || isInverse;
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
