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
        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        public NumericMinMaxValidateRule()
        {
            MinValue = 1;
            MaxValue = 255;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;
            var ErrorValidateResult = new ValidationResult(
                false,
                $"必须{MinValue}到{MaxValue}之间的整数"
            );
            if (string.IsNullOrEmpty(input))
            {
                return ErrorValidateResult;
            }
            if (!int.TryParse(input, out int num))
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
