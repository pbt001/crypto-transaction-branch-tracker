using CryptoBranchTracker.Objects.Classes;
using CryptoBranchTracker.WPF.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interaction logic for ctrlBranch.xaml
    /// </summary>
    public partial class ctrlBranch : UserControl
    {
        public Branch Branch { get; set; } = new Branch();

        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        public ctrlBranch()
        {
            InitializeComponent();
        }

        private void LoadTransactionData()
        {
            try
            {
                Transaction firstTransaction = this.Transactions.FirstOrDefault();
                Transaction secondTransaction = this.Transactions.ElementAtOrDefault(1);
                Transaction thirdTransaction = this.Transactions.ElementAtOrDefault(2);

                this.txtTransactionOne.Text = firstTransaction != null
                    ? $"• {firstTransaction.GetDisplayText()}"
                    : "";

                this.txtTransactionTwo.Text = secondTransaction != null
                    ? $"• {secondTransaction.GetDisplayText()}"
                    : "";

                this.txtTransactionThree.Text = thirdTransaction != null
                    ? $"• {thirdTransaction.GetDisplayText()}"
                    : "";
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred loading transaction data: {ex}");
            }
        }

        public void ImportTransactions(List<Transaction> lstTransactions)
        {
            try
            {
                this.Transactions = lstTransactions;
                this.LoadTransactionData();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred importing transactions: {ex}");
            }
        }

        public void RefreshDetails()
        {
            try
            {
                string resourceName;
                string displayName;

                if (string.IsNullOrWhiteSpace(this.Branch.Cryptocurrency))
                {
                    resourceName = "generic";
                    displayName = Strings.UNKNOWN_CRYPTO;
                }
                else
                {
                    //Using the replace because VS resource names can't include dollar signs
                    resourceName = this.Branch.Cryptocurrency.Replace("$", @"&#36;").ToLower();
                    displayName = $"{this.Branch.Cryptocurrency.ToUpper().Trim()} Branch";
                }

                this.txtCrypto.Text = displayName;
                this.imgCrypto.Source = Globals.GetResourceImage(resourceName);

                this.txtDates.Text = this.Branch.DateCreated.HasValue
                    ? $"{this.Branch.DateCreated.Value.ToShortDateString()} - Present"
                    : "N/A";
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred refreshing branch control details: {ex}");
            }
        }

        public ctrlBranch(Branch branch)
        {
            try
            {
                InitializeComponent();
                this.Branch = branch;

                this.RefreshDetails();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred initializing control: {ex}");
            }
        }
    }
}
