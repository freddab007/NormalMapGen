using Microsoft.Win32;
using System.IO;
using System.Windows;

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

                using (StreamReader s = new StreamReader(openFileDialog.FileName))
                {
                    
                }
            }
        }
    }
}
