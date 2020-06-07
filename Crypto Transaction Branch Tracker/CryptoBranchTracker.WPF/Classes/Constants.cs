using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CryptoBranchTracker.WPF.Classes
{
    public class Constants
    {
        public struct TransactionColours
        {
            public static SolidColorBrush DepositBrush = new SolidColorBrush(Color.FromRgb(108, 203, 86));

            public static SolidColorBrush WithdrawalBrush = new SolidColorBrush(Color.FromRgb(255, 105, 57));

            public static SolidColorBrush TradeBrush = new SolidColorBrush(Color.FromRgb(14, 125, 255));

            public static SolidColorBrush TransferBrush = new SolidColorBrush(Color.FromRgb(133, 97, 197));
        }
    }
}
