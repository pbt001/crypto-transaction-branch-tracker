using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Shell;

/*
 * https://blog.magnusmontin.net/2014/11/30/disabling-or-hiding-the-minimize-maximize-or-close-button-of-a-wpf-window/
 * 
 * That site provided the code to essentially disable the default minimize button on a window, so that I can provide my own without any difficulties
 * 
 * Without this code, the window title bar acts strange due to the fact that the buttons are still actually functional, just not visible.
 */

namespace CryptoBranchTracker.WPF.Classes
{
    public class BaseWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_STYLE = -16;
        private const int WS_MINIMIZEBOX = 0x20000;

        public BaseWindow()
        {
            try
            {
                WindowChrome.SetWindowChrome(this, new WindowChrome() { CaptionHeight = 34 });
                this.SourceInitialized += MainWindow_SourceInitialized;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred initializing the window: {ex}");
            }
        }

        private IntPtr _windowHandle;
        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            _windowHandle = new WindowInteropHelper(this).Handle;

            DisableMinimizeButton();
        }

        protected void DisableMinimizeButton()
        {
            if (_windowHandle == IntPtr.Zero)
                throw new InvalidOperationException("The window has not yet been completely initialized");

            SetWindowLong(_windowHandle, GWL_STYLE, GetWindowLong(_windowHandle, GWL_STYLE) & ~WS_MINIMIZEBOX);
        }
    }
}
