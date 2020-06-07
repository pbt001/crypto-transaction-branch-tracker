using CryptoBranchTracker.WPF.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CryptoBranchTracker.WPF.Classes
{
    public class Globals
    {
        public static ImageSource GetResourceImage(string name)
        {
            ImageSource src;

            try
            {
                ResourceManager rMan = Resources.ResourceManager;
                src = ((Bitmap)rMan.GetObject(name)).ToImageSource();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred getting resource image", ex);
            }

            return src;
        }
    }
}
