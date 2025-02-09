using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DynamicMovement
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.AppWindow.Resize(new SizeInt32(1200, 800));
            this.Closed += MainWindow_Closed;
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            App.active = !App.active;
            if (App.active)
            {
                myButton.Content = "Stop";
            }
            else
            {
                myButton.Content = "Start";
            }
        }

        public void updateText(string text)
        {
            myTextBlock.Text = text;
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            KeyboardHandler.Dispose();
        }


    }
}
