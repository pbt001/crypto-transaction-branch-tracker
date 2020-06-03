using CryptoBranchTracker.WFA.Classes;
using CryptoBranchTracker.WFA.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CryptoBranchTracker.WFA.Forms
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                List<Branch> lstBranches = Branch.GetAllLocalBranches();

                foreach (Branch branch in lstBranches)
                {
                    this.flpContainer.Controls.Add(new ctrlBranch(branch));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnNewBranch_Click(object sender, EventArgs e)
        {
            try
            {
                new Branch() { DateCreated = DateTime.Now.Date }.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
