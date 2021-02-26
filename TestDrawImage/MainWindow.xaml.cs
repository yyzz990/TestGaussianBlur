using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace TestDrawImage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            InitWindow();
        }

        private void InitWindow()
        {
            var stream = File.OpenRead("Tulips.jpg");
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();

            LeftImage.Source = bitmapImage;
        }

        private unsafe void DrawImage()
        {
            var writeableBitmap = new WriteableBitmap(LeftImage.Source as BitmapImage);

            for (int i = 1; i < writeableBitmap.PixelWidth; i++)
            {
                for (int j = 1; j < writeableBitmap.PixelHeight; j++)
                {
                    var color = writeableBitmap.GetPixel(i, j);
                    color = Color.FromArgb(color.A, (byte)(color.R + 50), color.G, color.B);
                    writeableBitmap.SetPixel(i, j, color);
                }
            }

            RightImage.Source = writeableBitmap;
        }

        private void SetPixel(byte[] byteList, int startIndex, Color color)
        {
            byteList[startIndex] = color.R;
            byteList[startIndex + 1] = color.G;
            byteList[startIndex + 2] = color.B;
            byteList[startIndex + 3] = color.A;
        }

        private unsafe Color GetPixelColor(byte* colorArr, int startIndex)
        {
            var blue = colorArr[startIndex];
            var green = colorArr[startIndex + 1];
            var red = colorArr[startIndex + 2];
            var alpha = colorArr[startIndex + 3];
            return Color.FromArgb(alpha,
                red,
                green,
                blue);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DrawImage();
        }
    }
}
