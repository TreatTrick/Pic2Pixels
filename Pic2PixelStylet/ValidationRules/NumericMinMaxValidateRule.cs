using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Pic2PixelStylet.ValidationRules
{
    internal class NumericMinMaxValidateRule : ValidationRule
    {
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public string ErrorMessage { get; set; } //like "Value must be between {0} and {1}"

        public NumericMinMaxValidateRule()
        {
            MinValue = 1;
            MaxValue = 255;
            ErrorMessage = "Value must be between {0} and {1}";
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;
            var ErrorValidateResult = new ValidationResult(
                false,
                string.Format(ErrorMessage, MinValue, MaxValue)
            );
            if (string.IsNullOrEmpty(input))
            {
                return ErrorValidateResult;
            }
            if (!double.TryParse(input, out double num))
            {
                return ErrorValidateResult;
            }
            if (num < MinValue || num > MaxValue)
            {
                return ErrorValidateResult;
            }
            return ValidationResult.ValidResult;
        }
    }
}
