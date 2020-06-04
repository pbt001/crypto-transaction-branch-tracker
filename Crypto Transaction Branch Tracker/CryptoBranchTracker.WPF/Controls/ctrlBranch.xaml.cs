using CryptoBranchTracker.Objects.Classes;
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
    /// Interaction logic for ctrlBranch.xaml
    /// </summary>
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
                InitializeComponent();
                this.Branch = branch;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred initializing control: {ex}");
            }
        }
    }
}
