using CryptoBranchTracker.WPF.Properties;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
                src = ((System.Drawing.Bitmap)rMan.GetObject(name)).ToImageSource();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred getting resource image", ex);
            }

            return src;
        }

        public static void RefreshSettings()
        {
            try
            {
                PaletteHelper helper = new PaletteHelper();
                ITheme theme = helper.GetTheme();

                theme.SetBaseTheme(Constants.Settings.DarkMode ? Theme.Dark : Theme.Light);
                theme.SetPrimaryColor(Constants.Settings.ColourScheme);

                //theme.SetPrimaryColor(SwatchHelper.Lookup[(MaterialDesignColor)PrimaryColor.Amber]);
                
                helper.SetTheme(theme);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred refreshing settings: {ex}");
            }
        }
    }
}
