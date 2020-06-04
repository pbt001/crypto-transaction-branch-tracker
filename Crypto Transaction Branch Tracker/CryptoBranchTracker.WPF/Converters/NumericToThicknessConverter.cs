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
    public sealed class NumericToThicknessConverter : IValueConverter
    {
        public ThicknessTypes ThicknessType { get; set; } = ThicknessTypes.ALL;

        public double DefaultMarginValue { get; set; } = 0;

        public enum ThicknessTypes
        {
            ALL,
            TOP,
            LEFT,
            RIGHT,
            BOTTOM
        }

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is double dblValue)
                {
                    switch (this.ThicknessType)
                    {
                        case ThicknessTypes.TOP:
                            return new Thickness(this.DefaultMarginValue, dblValue, this.DefaultMarginValue, this.DefaultMarginValue);
                        case ThicknessTypes.LEFT:
                            return new Thickness(dblValue, this.DefaultMarginValue, this.DefaultMarginValue, this.DefaultMarginValue);
                        case ThicknessTypes.RIGHT:
                            return new Thickness(this.DefaultMarginValue, this.DefaultMarginValue, dblValue, this.DefaultMarginValue);
                        case ThicknessTypes.BOTTOM:
                            return new Thickness(this.DefaultMarginValue, this.DefaultMarginValue, this.DefaultMarginValue, dblValue);
                        default:
                            return new Thickness(dblValue);
                    }
                }
                else
                    return new Thickness(this.DefaultMarginValue);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred converting to thickness: {ex}");
            }
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is Thickness thcValue)
                {
                    switch (this.ThicknessType)
                    {
                        case ThicknessTypes.TOP:
                        case ThicknessTypes.ALL:
                            return thcValue.Top;
                        case ThicknessTypes.LEFT:
                            return thcValue.Left;
                        case ThicknessTypes.RIGHT:
                            return thcValue.Right;
                        case ThicknessTypes.BOTTOM:
                            return thcValue.Bottom;
                        default:
                            return this.DefaultMarginValue;
                    }
                }
                else
                    return this.DefaultMarginValue;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred converting from thickness: {ex}");
            }
        }
    }
}
