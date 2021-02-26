using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace TestDrawImage
{
    public static class BitmapUtils
    {
        public unsafe static Color GetPixel(this WriteableBitmap bitmap, int x, int y)
        {
            if (x < 0 || y < 0 || x >= bitmap.PixelWidth || y >= bitmap.PixelHeight)
                return Color.Empty;
            bitmap.Lock();
            var backBuffer = (byte*)bitmap.BackBuffer;
            var index = (y * bitmap.PixelWidth + x) * (bitmap.Format.BitsPerPixel / 8);
            var color = Color.FromArgb(backBuffer[index + 3],
                backBuffer[index], 
                backBuffer[index + 1],
                backBuffer[index + 2]);
            bitmap.Unlock();

            return color;
        }

        public unsafe static void SetPixel(this WriteableBitmap bitmap, int x, int y, Color color)
        {
            if (x < 0 || y < 0 || x >= bitmap.PixelWidth || y >= bitmap.PixelHeight)
                return;
            bitmap.Lock();
            var backBuffer = (byte*)bitmap.BackBuffer;
            var index = (y * bitmap.PixelWidth + x) * (bitmap.Format.BitsPerPixel / 8);
            backBuffer[index] = color.R;
            backBuffer[index + 1] = color.G;
            backBuffer[index + 2] = color.B;
            backBuffer[index + 3] = color.A;
            bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
            bitmap.Unlock();
        }
    }
}
