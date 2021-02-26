using System;
using System.Diagnostics;
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

            DrawImage();
        }

        private unsafe void DrawImage()
        {
            var writeableBitmap = new WriteableBitmap(LeftImage.Source as BitmapImage);

            for (int i = 1; i < writeableBitmap.PixelWidth; i++)
            {
                for (int j = 1; j < writeableBitmap.PixelHeight; j++)
                {
                    var p = writeableBitmap.GetPixel(i, j);
                    var s = Sobel(writeableBitmap, i, j);
                    var color = BitmapUtils.LerpColor(Color.Black, p, s);
                    //if (i < 10 && j < 10)
                    //    Debug.WriteLine(p + "  " + s);
                    // color = Color.FromArgb(color.A, (byte)(color.R), color.G, color.B);
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

        public struct Pos
        {
            public Pos(int x,int y)
            {
                X = x;
                Y = y;
            }
            public int X;
            public int Y;
        }


        public Pos[] PosArr = new Pos[]
        {
            new Pos(-1,-1), new Pos(0,-1), new Pos(1,-1),
            new Pos(-1, 0), new Pos(0, 0), new Pos(1, 0),
            new Pos(-1, 1), new Pos(0, 1), new Pos(1,  1),
        };
        public float Sobel(WriteableBitmap bitmap, int x, int y)
        {
            var filterX = new int[] {
                -1, 0, 1,
                -2, 0, 2,
                -1, 0, 1
            };
            var filterY = new int[] {
                -1, -2, -1,
                0, 0, 0,
                1, 2, 1
            };

            float luminance = 0;
            float edgeX = 0;
            float edgeY = 0;
            for (int i = 0; i < 9; i++)
            {
                var pos = PosArr[i];
                var pixel = bitmap.GetPixel(x + pos.X, y + pos.Y);
                luminance = BitmapUtils.Luminance(pixel);
                edgeX += luminance * filterX[i];
                edgeX += luminance * filterY[i];
            }

            float edge = 1 - Math.Abs(edgeX) - Math.Abs(edgeY);
            edge = Math.Clamp(edge, 0, 1);
            return edge;
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
