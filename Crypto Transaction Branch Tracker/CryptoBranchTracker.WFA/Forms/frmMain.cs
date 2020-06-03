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
            Branch n = new Branch("cwpy9HP2iPd0cfUL8XTzdA2yTjVPMzc0M0jRNU8xTdU1sUgx0k0yN0nVNUgzNkkzN0hLM042q3GCaPPzD3ENtna2yPP0cYt0zM8OdivzC6rMSAzOrPCpynZxDXUEAA==");
        }
    }
}
