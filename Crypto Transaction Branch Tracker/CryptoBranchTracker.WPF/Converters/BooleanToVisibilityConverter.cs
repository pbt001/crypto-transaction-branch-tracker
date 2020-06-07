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
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; } = Visibility.Visible;

        public Visibility FalseValue { get; set; } = Visibility.Collapsed;

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
                throw new Exception($"An error occurred converting to visibility: {ex}");
            }
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            try
            {
                return value is Visibility visValue
                    ? visValue == this.TrueValue
                    : false;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred converting to boolean: {ex}");
            }
        }
    }
}
