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
using System.Reflection.Emit;
using System.Configuration;

namespace CryptoBranchTracker.WFA.Controls
{
    public partial class ctrlBranch : GroupBox
    {
        public Branch Branch { get; set; } = new Branch();

        public ctrlBranch()
        {
            InitializeComponent();
        }

        private void RefreshDetails()
        {
            try
            {
                //Don't want to be dealing with totally empty branches really
                if (this.Branch.Identifier != Guid.Empty)
                {
                    this.Text = this.Branch.DateCreated.HasValue ? this.Branch.DateCreated.Value.ToLongDateString() : "Unknown";
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred refreshing details: {ex}");
            }
        }

        public ctrlBranch(Branch branch)
        {
            try
            {
                this.Branch = branch;
                this.RefreshDetails();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred initializing the branch control: {ex}");
            }
        }
    }
}
