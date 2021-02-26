using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace TestDrawImage
{
    public static class BitmapUtils
    {

        public unsafe static Color GetPixel(this WriteableBitmap bitmap, int x, int y)
        {
            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if (x > bitmap.PixelWidth - 1) x = bitmap.PixelWidth - 1;
            if (y > bitmap.PixelHeight - 1) y = bitmap.PixelHeight - 1;

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
            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if (x > bitmap.PixelWidth - 1) x = bitmap.PixelWidth - 1;
            if (y > bitmap.PixelHeight - 1) y = bitmap.PixelHeight - 1;

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


        public static float Luminance(Color color)
        {
            return 0.2125f * color.R / 255f + 0.7154f * color.G / 255f + 0.0721f * color.B / 255f;
        }




        public static Color LerpColor(Color from, Color to, float lerp)
        {
            lerp = Math.Clamp(lerp, 0, 1);
            float r = from.R + (to.R - from.R) * lerp;
            float g = from.G + (to.G - from.G) * lerp;
            float b = from.B + (to.B - from.B) * lerp;
            float a = from.A + (to.A - from.A) * lerp;

            return Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);

        }
    }



}
