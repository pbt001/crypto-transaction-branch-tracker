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

        public ctrlBranch()
        {
            InitializeComponent();
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
