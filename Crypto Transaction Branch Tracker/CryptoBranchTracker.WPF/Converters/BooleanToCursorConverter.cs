using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace CryptoBranchTracker.WPF.Converters
{
    public class BooleanToCursorConverter : IValueConverter
    {
        public Cursor TrueValue { get; set; } = Cursors.Arrow;

        public Cursor FalseValue { get; set; } = Cursors.Hand;

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
                throw new Exception($"An error occurred converting to cursor: {ex}");
            }
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            try
            {
                return value is Cursor cursor
                    ? cursor == this.TrueValue
                    : false;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred converting to cursor: {ex}");
            }
        }
    }
}
