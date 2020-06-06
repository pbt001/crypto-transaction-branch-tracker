using CryptoBranchTracker.Objects.Classes;
using CryptoBranchTracker.WPF.Classes;
using CryptoBranchTracker.WPF.Controls;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
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

        private void LoadBranches()
        {
            try
            {
                this.ugBranches.Children.Clear();

                List<Branch> lstBranches = Branch.GetAllLocalBranches().
                    OrderByDescending (x => x.DateCreated).
                    ThenByDescending(x => x.TimeCreated).ToList();

                List<Transaction> lstTransactions = Transaction.GetAllLocalTransactions();

                foreach (Branch branch in lstBranches)
                {
                    ctrlBranch curBranch = new ctrlBranch(branch);

                    curBranch.RequestEdit += CurBranch_RequestEdit;
                    curBranch.RequestDelete += CurBranch_RequestDelete;

                    curBranch.ImportTransactions(
                            lstTransactions.
                                Where(x => x.BranchIdentifier == branch.Identifier).ToList()
                        );

                    this.ugBranches.Children.Add(curBranch);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred loading branches: {ex}");
            }
        }

        private void CurBranch_RequestDelete(object sender, EventArgs e)
        {
            try
            {
                if (sender is ctrlBranch curBranch)
                {
                    curBranch.Branch.Delete();
                    this.LoadBranches();
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
                this.dhCryptocurrency.IsOpen = true;
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
                    }

                    this.LoadBranches();
                    this.dhCryptocurrency.DataContext = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                this.dhMenu.IsLeftDrawerOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                this.dhMenu.IsLeftDrawerOpen = false;
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
                this.tbMenu.IsChecked = false;
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
                this.tcMain.SelectedItem = this.tiTransactions;
                this.tbMenu.IsChecked = false;
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
                this.tcMain.SelectedItem = this.tiKey;
                this.tbMenu.IsChecked = false;
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
                this.dhCryptocurrency.IsOpen = true;
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
    }
}
