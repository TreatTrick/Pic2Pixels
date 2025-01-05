using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Pic2PixelStylet.Converters
{
    internal class MouseEventArgsToPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not System.Windows.Input.MouseEventArgs args)
                throw new ArgumentException(
                    "Value must be of type System.Windows.Input.MouseEventArgs"
                );
            if (parameter is not System.Windows.IInputElement inputElement)
                throw new ArgumentException(
                    "Parameter must be of type System.Windows.IInputElement"
                );
            return args.GetPosition(inputElement);
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            throw new NotImplementedException();
        }
    }
}
