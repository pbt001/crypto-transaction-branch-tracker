﻿using System;
using System.Collections.Generic;
using System.Linq;
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
            Console.WriteLine(Branch.GetAllLocalBranches().First().Transactions.Count.ToString());

            Console.ReadLine();
        }
    }
}
