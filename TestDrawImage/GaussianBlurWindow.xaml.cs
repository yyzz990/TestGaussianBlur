using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Linq;
namespace TestDrawImage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GaussianBlurWindow : Window
    {
        public GaussianBlurWindow()
        {
            InitializeComponent();

            InitGaussianKernel();

            InitWindow();
        }

        private void InitWindow()
        {
            var stream = File.OpenRead("Koala.jpg");
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
            LeftImage.Source = bitmapImage;

            DrawImage();
        }

        #region  描边


        private unsafe void DrawImage()
        {
            var writeableBitmap = new WriteableBitmap(LeftImage.Source as BitmapImage);
            var colorList = new Color[writeableBitmap.PixelWidth * writeableBitmap.PixelHeight];
            for (int i = 0; i < writeableBitmap.PixelWidth; i++)
            {
                for (int j = 0; j < writeableBitmap.PixelHeight; j++)
                {
                    var color = GaussianBlur(writeableBitmap, i, j);
                    var index = j * writeableBitmap.PixelWidth + i;
                    colorList[index] = color;
                }
            }

            var colorBytes = colorList.SelectMany(p => new byte[] { p.R, p.G, p.B, p.A }).ToArray();
            writeableBitmap.Lock();
            var drawRect = new Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight);
            writeableBitmap.WritePixels(drawRect, colorBytes, writeableBitmap.BackBufferStride, 0);
            writeableBitmap.AddDirtyRect(drawRect);
            writeableBitmap.Unlock();

            RightImage.Source = writeableBitmap;
        }

        public struct Pos
        {
            public Pos(int x, int y)
            {
                X = x;
                Y = y;
            }
            public int X;
            public int Y;
        }

        public int filterSize = 3;
        public float[] filterValues;
        public Pos[] PosArr;

        //初始化高斯核
        public void InitGaussianKernel()
        {
            PosArr = new Pos[filterSize * filterSize];
            filterValues = new float[filterSize * filterSize];

            int center = (int)(filterSize / 2f);
            float sum = 0;
            //计算位置与权重
            for (int x = 0; x < filterSize; x++)
            {
                for (int y = 0; y < filterSize; y++)
                {
                    var pos = new Pos(x - center, y - center);
                    var index = y * filterSize + x;
                    PosArr[index] = pos;
                    filterValues[index] = GetGaussianValue(pos.X, pos.Y);
                    sum += filterValues[index];

                   // Debug.Write(string.Format("({0},{1})", pos.X, pos.Y));

                }
              //  Debug.WriteLine("");
            }
            //归一
            for (int i = 0; i < filterValues.Length; i++)
            {
                filterValues[i] = filterValues[i] / sum;
            }

        }

        //gaussian function
        public float GetGaussianValue(int x, int y)
        {
            float sigma = 1;
            double n = 1f / (2 * Math.PI * sigma * sigma) * Math.Exp(-(x * x + y * y) / (2 * sigma * sigma));
            return (float)n;
        }



        public Color GaussianBlur(WriteableBitmap bitmap, int x, int y)
        {
            float r = 0;
            float g = 0;
            float b = 0;
            float a = 0;

            for (int i = 0; i < PosArr.Length; i++)
            {
                var p = PosArr[i];
                var color = bitmap.GetPixel(x + p.X, y + p.Y);
                var w = filterValues[i];
                r += color.R * w;
                g += color.G * w;
                b += color.B * w;
                a += color.A * w;
            }

            var rc = Color.FromArgb(
                (int)(a),
                (int)(r),
                (int)(g),
                (int)(b)
            );
            var cc = bitmap.GetPixel(x , y );
            return rc;
        }

        private int testN = 0;

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DrawImage();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsInitialized)
                return;
            int n = (int)e.NewValue;
            int newSize = (int) (n / 2) * 2 + 1;
            if (filterSize != newSize)
            {
                filterSize = newSize;
                InitGaussianKernel();
                Debug.WriteLine("Slider_ValueChanged " + filterSize);
                DrawImage();
            }

        }
    }
}
