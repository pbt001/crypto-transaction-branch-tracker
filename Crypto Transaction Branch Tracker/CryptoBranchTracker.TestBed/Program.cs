using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using CryptoBranchTracker.Objects.Classes;

namespace CryptoBranchTracker.TestBed
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}
