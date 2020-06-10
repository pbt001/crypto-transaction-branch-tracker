using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CryptoBranchTracker.Objects.Classes;

namespace CryptoBranchTracker.TestBed
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            new Branch()
            {
                Identifier = Guid.NewGuid(),
                Cryptocurrency = "BCH"
            }.Save();

            Console.ReadLine();
        }
    }
}
