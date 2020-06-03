using CryptoBranchTracker.WFA.Classes;
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
            Transaction transact = new Transaction("Cwly9At2dA7x9PeLdwKynT2sDaBAF0SjETC5mhAkfW6ejiHWBihCIZEBrtZOoZEogn7+Ia7B1s4WOZ4+oaZhwc4V3mHBWf5VBoZh3q7ZJT45Hp6V5SnBjo62tijaXBxDXK3NjM2NzMwtLQ0szM0MjY2MTU1Q1IR4+rpaG5uZGUIdCAA=");
        }
    }
}
