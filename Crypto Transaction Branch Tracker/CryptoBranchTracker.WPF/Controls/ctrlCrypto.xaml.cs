using CryptoBranchTracker.WPF.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CryptoBranchTracker.WPF.Controls
{
    /// <summary>
    /// Interaction logic for ctrlCrypto.xaml
    /// </summary>
    public partial class ctrlCrypto : UserControl
    {
        public DictionaryEntry CryptoSet { get; set; } = new DictionaryEntry();

        public ctrlCrypto()
        {
            InitializeComponent();
        }

        private void RefreshDetails()
        {
            try
            {
                this.lblCurrency.Content = this.CryptoSet.Key.ToString().Replace(@"&#36;", "$").ToUpper();
                this.imgIcon.Source = ((Bitmap)this.CryptoSet.Value).ToImageSource();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred refreshing details: {ex}");
            }
        }

        public ctrlCrypto(DictionaryEntry cryptoSet)
        {
            try
            {
                InitializeComponent();
                this.CryptoSet = cryptoSet;

                this.RefreshDetails();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred creating control instance: {ex}");
            }
        }
    }
}
