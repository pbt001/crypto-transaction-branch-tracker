using CryptoBranchTracker.Objects.Classes;
using CryptoBranchTracker.WPF.Classes;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for ctrlTransaction.xaml
    /// </summary>
    public partial class ctrlTransaction : UserControl
    {
        public Transaction Transaction { get; set; } = new Transaction();

        public ImageSource CryptoResource { get; set; }

        public string CryptoString { get; set; } = "";

        public ctrlTransaction()
        {
            InitializeComponent();
        }

        public ctrlTransaction(Transaction transaction, ImageSource cryptoSource, string cryptoString)
        {
            try
            {
                InitializeComponent();

                this.Transaction = transaction;
                this.CryptoResource = cryptoSource;
                this.CryptoString = cryptoString.ToUpper().Trim();

                this.RefreshDetails();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred initiliazing control: {ex}");
            }
        }

        private void RefreshDetails()
        {
            try
            {
                this.txtSource.Text = "N/A";
                this.txtDestination.Text = "N/A";

                this.txtDate.Text = this.Transaction.DateProcessed.HasValue
                    ? this.Transaction.DateProcessed.Value.ToString("MMMM d, yyyy")
                    : "N/A";

                this.txtTime.Text = this.Transaction.TimeProcessed.HasValue
                    ? new DateTime(this.Transaction.TimeProcessed.Value.Ticks).ToString("h:mm tt")
                    : "N/A";

                switch (this.Transaction.TransactionType)
                {
                    case Transaction.TransactionTypes.DEPOSIT:
                        this.piIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowDown;
                        this.piIcon.Foreground = Constants.TransactionColours.DepositBrush;
                        break;
                    case Transaction.TransactionTypes.TRADE:
                        this.piIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.SwapHorizontal;
                        this.piIcon.Foreground = Constants.TransactionColours.TradeBrush;
                        break;
                    case Transaction.TransactionTypes.TRANSFER:
                        this.piIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ChevronDoubleRight;
                        this.piIcon.Foreground = Constants.TransactionColours.TransferBrush;
                        break;
                    case Transaction.TransactionTypes.WITHDRAWAL:
                        this.piIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowUp;
                        this.piIcon.Foreground = Constants.TransactionColours.WithdrawalBrush;
                        break;
                }

                this.imgSourceCrypto.Visibility = Visibility.Collapsed;
                this.piSourceIcon.Visibility = Visibility.Visible;

                switch (this.Transaction.Source)
                {
                    case Transaction.LocationTypes.BANK:
                        this.piSourceIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Bank;
                        break;
                    case Transaction.LocationTypes.CRYPTO:
                        this.imgSourceCrypto.Visibility = Visibility.Visible;
                        this.piSourceIcon.Visibility = Visibility.Collapsed;

                        this.imgSourceCrypto.Source = this.CryptoResource;
                        this.txtSource.Text = $"- {this.CryptoString}";
                        break;
                    case Transaction.LocationTypes.FIAT:
                        this.piSourceIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Money;

                        this.txtSource.Text = $"- {this.Transaction.FiatDifference}";
                        break;
                    case Transaction.LocationTypes.WALLET:
                        this.piSourceIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Wallet;
                        break;
                }

                this.imgDestinationCrypto.Visibility = Visibility.Collapsed;
                this.piDestinationIcon.Visibility = Visibility.Visible;

                switch (this.Transaction.Destination)
                {
                    case Transaction.LocationTypes.BANK:
                        this.piDestinationIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Bank;
                        break;
                    case Transaction.LocationTypes.CRYPTO:
                        this.imgDestinationCrypto.Visibility = Visibility.Visible;
                        this.piDestinationIcon.Visibility = Visibility.Collapsed;

                        this.imgDestinationCrypto.Source = this.CryptoResource;
                        this.txtDestination.Text = $"+ {this.CryptoString}";
                        break;
                    case Transaction.LocationTypes.FIAT:
                        this.piDestinationIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Money;

                        this.txtDestination.Text = $"+ {this.Transaction.FiatDifference}";
                        break;
                    case Transaction.LocationTypes.WALLET:
                        this.piDestinationIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Wallet;
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred refreshing details: {ex}");
            }
        }
    }
}
