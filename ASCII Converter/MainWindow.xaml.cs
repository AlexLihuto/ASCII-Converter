using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using LibraryASCII;
using ASCIILibrary2;

namespace ASCII_Converter
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double WIDTH_OFFSET = 2.0;
        private const int MAX_WIDTH = 474;

        public MainWindow()
        {
            InitializeComponent();
        }
        [STAThread]
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Images | *.bmp; *.png; *.jpg; *.JPEG"
            };

            openFileDialog.ShowDialog();

            var bitmap1 = new BitmapImage(new Uri(openFileDialog.FileName));

            ImagePictureBox.Source = bitmap1 as ImageSource;

            if (ImagePictureBox.Source == bitmap1 as ImageSource)  //условие, которое выводит информацию о загружаемой картинке в textbox
                OriginImageInfo.Text = $"{bitmap1.Format}\n";
                OriginImageInfo.Text = $"{bitmap1.PixelHeight}*{bitmap1.PixelWidth}\n" + $"{bitmap1.UriSource}\n"; ;

            var bitmap = new Bitmap(openFileDialog.FileName);
            bitmap = ResizeBitmap(bitmap);
            bitmap.ToGrayscale();

            var converter = new BitmapToASCIIConverter(bitmap);
            var rows = converter.Convert();

            foreach (var row in rows)
                Console.WriteLine(row);

            var rowsNegative = converter.ConvertAsNegative();
            File.WriteAllLines("image.txt", rowsNegative.Select(r => new string(r)));
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            TextBox.Text = File.ReadAllText("image.txt");
        }

        
        private float Map(float valueToMap, float start1, float stop1, float start2, float stop2)
        {
            return ((valueToMap - start1) / (stop1 - start1)) * (stop2 - start2) + start2;
        }

        private static Bitmap ResizeBitmap(Bitmap bitmap)
        {
            var newHeight = bitmap.Height / WIDTH_OFFSET * MAX_WIDTH / bitmap.Width;
            if (bitmap.Width > MAX_WIDTH || bitmap.Height > newHeight)
                bitmap = new Bitmap(bitmap, new System.Drawing.Size(MAX_WIDTH, (int)newHeight));
            return bitmap;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog s = new System.Windows.Forms.SaveFileDialog();
            s.Filter = "txt files (.txt)|.txt|All files (.)|.";
            if (s.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllText(s.FileName, TextBox.Text, Encoding.Default);
            }
        }

    }
}
