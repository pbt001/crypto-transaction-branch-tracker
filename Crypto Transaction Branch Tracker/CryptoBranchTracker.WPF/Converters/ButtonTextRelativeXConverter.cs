using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace CryptoBranchTracker.WPF.Converters
{
    public sealed class ButtonTextRelativeXConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is TextBlock ctrlOrigin && (ctrlOrigin.Parent as FrameworkElement).Parent is Button btnParent)
                {
                    return ctrlOrigin.TransformToAncestor(btnParent).Transform(new Point(0, 0)).X + btnParent.Margin.Left + btnParent.Margin.Right;
                }
                else
                    return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            throw new Exception("Error: converter is one-way only");
        }
    }
}
