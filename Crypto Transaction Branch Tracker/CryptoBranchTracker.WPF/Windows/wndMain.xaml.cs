﻿using CryptoBranchTracker.Objects.Classes;
using CryptoBranchTracker.WPF.Classes;
using CryptoBranchTracker.WPF.Controls;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CryptoBranchTracker.WPF.Windows
{
    /// <summary>
    /// Interaction logic for wndMain.xaml
    /// </summary>
    public partial class wndMain : BaseWindow
    {
        public wndMain()
        {
            InitializeComponent();
        }

        public IEnumerable<ISwatch> Swatches { get; } = SwatchHelper.Swatches;

        private void LoadBranches()
        {
            try
            {
                this.ugBranches.Children.Clear();

                List<Branch> lstBranches = Branch.GetAllLocalBranches().
                    OrderByDescending (x => x.DateCreated).
                    ThenByDescending(x => x.TimeCreated).ToList();

                if (lstBranches.Any())
                {
                    this.gridEmpty.Visibility = Visibility.Collapsed;
                    this.ugBranches.Visibility = Visibility.Visible;
                }
                else
                {
                    this.gridEmpty.Visibility = Visibility.Visible;
                    this.ugBranches.Visibility = Visibility.Collapsed;
                }

                foreach (Branch branch in lstBranches)
                {
                    ctrlBranch curBranch = new ctrlBranch(branch);

                    curBranch.RequestEdit += CurBranch_RequestEdit;
                    curBranch.RequestDelete += CurBranch_RequestDelete;
                    curBranch.RequestAddTransaction += CurBranch_RequestAddTransaction;
                    curBranch.RequestTransactionView += CurBranch_RequestTransactionView;

                    this.ugBranches.Children.Add(curBranch);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred loading branches: {ex}");
            }
        }

        private void ReadInTransactions(ctrlBranch branch)
        {
            try
            {
                this.pnlTransactions.Children.Clear();

                this.tiBranchTransactions.DataContext = branch;
                this.tcMain.SelectedItem = this.tiBranchTransactions;

                this.txtCrypto.Text = branch.DisplayName;
                this.imgCrypto.Source = branch.Resource;

                if (branch.Branch.DateCreated.HasValue)
                    this.txtCrypto.Text += $" - {branch.Branch.DateCreated.Value.ToShortDateString()}";

                foreach (Transaction transaction in branch.Branch.Transactions)
                {
                    ctrlTransaction curTransaction = new ctrlTransaction(transaction, branch.Resource, branch.Branch.Cryptocurrency);

                    curTransaction.RequestDelete += (origin, ev) =>
                    {
                        Transaction localTransaction = (origin as ctrlTransaction).Transaction;
                        localTransaction.Delete();

                        branch.Branch.Transactions.Remove(
                                branch.Branch.Transactions.
                                    Where(x => x.Identifier == localTransaction.Identifier).FirstOrDefault()
                            );

                        branch.RefreshDetails();
                        this.pnlTransactions.Children.Remove(curTransaction);
                    };

                    this.pnlTransactions.Children.Add(curTransaction);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred reading in transactions: {ex}");
            }
        }

        private void ReloadSettings()
        {
            try
            {
                this.btnDarkMode.IsChecked = Constants.Settings.DarkMode;
                this.btnAutoMax.IsChecked = Constants.Settings.AutoMax;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred reloading settings: {ex}");
            }
        }

        private void CurBranch_RequestTransactionView(object sender, EventArgs e)
        {
            try
            {
                if (sender is ctrlBranch curBranchControl)
                    this.ReadInTransactions(curBranchControl);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void SubmitCurrentTransaction()
        {
            try
            {
                if (this.dhTransactionDetails.DataContext is ctrlBranch curBranch)
                {
                    DateTime dteNow = DateTime.Now;

                    Transaction transaction = new Transaction()
                    {
                        DateProcessed = dteNow.Date,
                        TimeProcessed = dteNow.TimeOfDay,
                        BranchIdentifier = curBranch.Branch.Identifier,
                        Identifier = Guid.NewGuid()
                    };

                    transaction.FiatDifference = double.TryParse(this.txtTransactionFiat.Text, out double dblFiat)
                        ? dblFiat
                        : -1;

                    if (this.cmbTransactionType.SelectedItem is ComboBoxItem curType)
                        transaction.TransactionType = (Transaction.TransactionTypes)curType.DataContext;

                    if (this.cmbTransactionFrom.SelectedItem is ComboBoxItem curSource)
                        transaction.Source = (Transaction.LocationTypes)curSource.DataContext;

                    if (this.cmbTransactionTo.SelectedItem is ComboBoxItem curDestination)
                        transaction.Destination = (Transaction.LocationTypes)curDestination.DataContext;

                    transaction.Save();
                    curBranch.Branch.ReloadTransactions();

                    curBranch.RefreshDetails();
                }

                this.dhTransactionDetails.IsOpen = false;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred submitting transaction: {ex}");
            }
        }

        private void CurBranch_RequestAddTransaction(object sender, EventArgs e)
        {
            try
            {
                this.dhTransactionDetails.DataContext = sender as ctrlBranch;
                this.OpenTransactionDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void CurBranch_RequestDelete(object sender, EventArgs e)
        {
            try
            {
                if (sender is ctrlBranch curBranch)
                {
                    curBranch.Branch.Delete();
                    this.ugBranches.Children.Remove(curBranch);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void CurBranch_RequestEdit(object sender, EventArgs e)
        {
            try
            {
                this.dhCryptocurrency.DataContext = (sender as ctrlBranch).Branch;
                this.OpenCryptoSearchDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Check the state of the window to determine the visibility of the maximize/restore title bar buttons
        /// </summary>
        private void CheckMaxRestore()
        {
            try
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.btnMaximize.Visibility = Visibility.Collapsed;
                    this.btnRestore.Visibility = Visibility.Visible;
                }
                else
                {
                    this.btnMaximize.Visibility = Visibility.Visible;
                    this.btnRestore.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred checking max/restore state: {ex}");
            }
        }

        private void OpenTransactionDialog()
        {
            try
            {
                this.cmbTransactionType.Text = Transaction.TransactionTypes.TRADE.ToString().FirstCharToUpper();
                this.cmbTransactionFrom.Text = Transaction.LocationTypes.FIAT.ToString().FirstCharToUpper();
                this.cmbTransactionTo.Text = Transaction.LocationTypes.CRYPTO.ToString().FirstCharToUpper();

                this.txtTransactionFiat.Clear();

                this.dhTransactionDetails.IsOpen = true;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred opening the transaction dialog: {ex}");
            }
        }

        private void OpenCryptoSearchDialog()
        {
            try
            {
                this.txtCryptoSearch.Text = "";
                this.dhCryptocurrency.IsOpen = true;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred opening the crypto search dialog: {ex}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SystemCommands.MinimizeWindow(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                SystemCommands.CloseWindow(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                SystemCommands.MaximizeWindow(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                SystemCommands.RestoreWindow(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void BaseWindow_StateChanged(object sender, EventArgs e)
        {
            try
            {
                this.CheckMaxRestore();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void BaseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.cmbTransactionType.Items.Clear();
                this.cmbTransactionFrom.Items.Clear();
                this.cmbTransactionTo.Items.Clear();

                this.CheckMaxRestore();
                this.LoadBranches();

                ResourceManager MyResourceClass = new ResourceManager(typeof(Properties.Resources));
                ResourceSet resourceSet = MyResourceClass.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

                foreach (DictionaryEntry entry in resourceSet.OfType<DictionaryEntry>().OrderBy(x => x.Key.ToString()))
                {
                    ctrlCrypto crypto = new ctrlCrypto(entry);
                    crypto.MouseLeftButtonUp += Crypto_MouseLeftButtonUp;

                    this.gridCurrencies.Children.Add(crypto);
                }

                foreach (Transaction.TransactionTypes type in Enum.GetValues(typeof(Transaction.TransactionTypes)))
                {
                    this.cmbTransactionType.Items.Add(
                            new ComboBoxItem()
                            {
                                DataContext = type,
                                Content = type.ToString().FirstCharToUpper()
                            }
                        );
                }

                foreach (Transaction.LocationTypes type in Enum.GetValues(typeof(Transaction.LocationTypes)))
                {
                    string strDisplayName = type.ToString().FirstCharToUpper();

                    this.cmbTransactionFrom.Items.Add(
                            new ComboBoxItem()
                            {
                                DataContext = type,
                                Content = strDisplayName
                            }
                        );

                    this.cmbTransactionTo.Items.Add(
                            new ComboBoxItem()
                            {
                                DataContext = type,
                                Content = strDisplayName
                            }
                        );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Crypto_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender is ctrlCrypto crypto) {
                    this.dhCryptocurrency.IsOpen = false;

                    this.txtCryptoSearch.Text = "";

                    if (this.dhCryptocurrency.DataContext is Branch brBranch)
                    {
                        brBranch.Cryptocurrency = crypto.CryptoSet.Key.ToString().ToUpper();
                        brBranch.Save();

                        ctrlBranch tarBranch = this.ugBranches.Children.OfType<ctrlBranch>().
                            Where(x => x.Branch.Identifier == brBranch.Identifier).FirstOrDefault();

                        if (tarBranch != null)
                            tarBranch.RefreshDetails();
                        else
                            this.LoadBranches();
                    }
                    else
                    {
                        DateTime dteNow = DateTime.Now;

                        Branch bSave = new Branch()
                        {
                            Cryptocurrency = crypto.CryptoSet.Key.ToString().ToUpper(),
                            DateCreated = dteNow.Date,
                            TimeCreated = dteNow.TimeOfDay,
                            Identifier = Guid.NewGuid()
                        };

                        bSave.Save();

                        this.LoadBranches();
                    }

                    this.dhCryptocurrency.DataContext = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnBranches_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.tcMain.SelectedItem = this.tiBranches;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            try
            {
                this.OpenCryptoSearchDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void txtCryptoSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //Without this layout update, a weird bug occurrs where a phantom selection is made, preventing any focus on the textbox
                if (string.IsNullOrWhiteSpace(this.txtCryptoSearch.Text))
                    this.txtCryptoSearch.UpdateLayout();

                foreach (ctrlCrypto crypto in this.gridCurrencies.Children)
                {
                    crypto.Visibility = (crypto.CryptoSet.Key.ToString().ToUpper().Contains(this.txtCryptoSearch.Text.ToUpper()))
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            try
            {
                this.dhTransactionDetails.IsOpen = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            try
            {
                this.SubmitCurrentTransaction();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void txtTransactionFiat_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Return)
                    this.SubmitCurrentTransaction();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Strings.GITHUB_URL);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            try
            {
                this.tcMain.SelectedItem = this.tiSettings;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void tcMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.tcMain.SelectedItem == this.tiSettings)
                    this.ReloadSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnDarkMode_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                bool btnChecked = this.btnDarkMode.IsChecked.Value;

                if (Constants.Settings.DarkMode != btnChecked)
                {
                    Constants.Settings.DarkMode = btnChecked;
                    Constants.Settings.Save();

                    Globals.RefreshSettings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            try
            {
                this.dhColours.IsOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btnCurrent && btnCurrent.DataContext is Color clrCurrent)
                {
                    Constants.Settings.ColourScheme = Color.FromArgb(clrCurrent.A, clrCurrent.R, clrCurrent.G, clrCurrent.B);
                    Constants.Settings.Save();

                    Globals.RefreshSettings();

                    this.dhColours.IsOpen = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            try
            {
                this.OpenCryptoSearchDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnAutoMax_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                Constants.Settings.AutoMax = this.btnAutoMax.IsChecked.Value;
                Constants.Settings.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
