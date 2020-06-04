using CryptoBranchTracker.WPF.Classes;
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
using System.Windows.Shapes;

namespace CryptoBranchTracker.WPF.Windows
{
    /// <summary>
    /// Interaction logic for wndMain.xaml
    /// </summary>
    public partial class wndMain : BaseWindow
    {
        public wndMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Check the state of the window to determine the visibility of the maximize/restore title bar buttons
        /// </summary>
        private void CheckMaxRestore()
        {
            try
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.btnMaximize.Visibility = Visibility.Collapsed;
                    this.btnRestore.Visibility = Visibility.Visible;
                }
                else
                {
                    this.btnMaximize.Visibility = Visibility.Visible;
                    this.btnRestore.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred checking max/restore state: {ex}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SystemCommands.MinimizeWindow(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                SystemCommands.CloseWindow(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                SystemCommands.MaximizeWindow(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                SystemCommands.RestoreWindow(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void BaseWindow_StateChanged(object sender, EventArgs e)
        {
            try
            {
                this.CheckMaxRestore();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void BaseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.CheckMaxRestore();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
