using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CryptoBranchTracker.WFA.Classes;

namespace CryptoBranchTracker.WFA.Controls
{
    public partial class ctrlBranch : UserControl
    {
        public Branch Branch { get; set; } = new Branch();

        public ctrlBranch()
        {
            InitializeComponent();
        }

        public ctrlBranch(Branch branch)
        {
            try
            {
                this.Branch = branch;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred initializing control: {ex}");
            }
        }

        private void RefreshDetails()
        {
            try
            {
                if (this.Branch.Identifier != Guid.Empty)
                {
                    this.gbMain.Text = this.Branch.DateCreated.HasValue ? this.Branch.DateCreated.Value.ToLongDateString() : "Unknown";
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred refreshing the details: {ex}");
            }
        }

        public void ImportTransactions(List<Transaction> lstTransactions)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred importing transactions: {ex}");
            }
        }

        private void ctrlBranch_Load(object sender, EventArgs e)
        {
            try
            {
                this.RefreshDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
