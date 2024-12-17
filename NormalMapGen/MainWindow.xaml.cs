using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace NormalMapGen
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        public void OpenImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPEG file (*.jpg/*jpeg)|*.jpg;*jpeg;*.jfif|PNG file (*.png)|*.png|BitMap file (*.bmp)|*.bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                ImageShow.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }
    }
}
