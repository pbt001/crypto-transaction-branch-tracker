using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CryptoBranchTracker.WPF.Converters
{
    public sealed class BooleanToOpacityConverter : IValueConverter
    {
        public double TrueValue { get; set; } = 1;

        public double FalseValue { get; set; } = 0.5;

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is bool blnValue)
                {
                    return blnValue
                        ? this.TrueValue
                        : this.FalseValue;
                }
                else
                    return this.FalseValue;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred converting to opacity: {ex}");
            }
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            try
            {
                return value is double dblValue
                    ? dblValue == this.TrueValue
                    : false;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred converting to boolean: {ex}");
            }
        }
    }
}
