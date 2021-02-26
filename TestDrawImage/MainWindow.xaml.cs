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

            DrawStrokeImage();
        }

        #region  描边

        public float m_EdgeOnly = 1;
        private unsafe void DrawStrokeImage()
        {
            var writeableBitmap = new WriteableBitmap(LeftImage.Source as BitmapImage);
            var colorList = new Color[writeableBitmap.PixelWidth * writeableBitmap.PixelHeight];
            for (int i = 0; i < writeableBitmap.PixelWidth; i++)
            {
                for (int j = 0; j < writeableBitmap.PixelHeight; j++)
                {
                    var p = writeableBitmap.GetPixel(i, j);
                    var s = Sobel(writeableBitmap, i, j);
                    var edgeColor = BitmapUtils.LerpColor(Color.White, p, s);//混合边缘颜色和原图颜色，edge越小，越判定为边缘
                    var backgroundColor = BitmapUtils.LerpColor(Color.White, Color.Black, s);//混合边缘颜色和背景颜色。
                    var color = BitmapUtils.LerpColor(edgeColor, backgroundColor, m_EdgeOnly);//原图混合还是混合背景颜色的插值。
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
                //读取像素的亮度
                luminance = BitmapUtils.Luminance(pixel);
                //对比边缘的值
                edgeX += luminance * filterX[i];
                edgeY += luminance * filterY[i];
            }
            //梯度值
            float edge = 1 - Math.Abs(edgeX) - Math.Abs(edgeY);
            edge = Math.Clamp(edge, 0, 1);
            return edge;
        }


        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DrawStrokeImage();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_EdgeOnly = (float)Math.Clamp(e.NewValue / 100f, 0, 1);
            Debug.WriteLine("Slider_ValueChanged " + e.NewValue);

        }
    }
}
