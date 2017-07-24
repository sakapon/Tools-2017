using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ImageResizer
{
    public static class BitmapUtility
    {
        public static Bitmap ResizeImage(Image source, Size size, InterpolationMode interpolationMode = InterpolationMode.Bilinear) =>
            ResizeImage(source, size.Width, size.Height, interpolationMode);

        public static Bitmap ResizeImage(Image source, int width, int height, InterpolationMode interpolationMode = InterpolationMode.Bilinear)
        {
            var bitmap = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.InterpolationMode = interpolationMode;
                graphics.DrawImage(source, 0, 0, width, height);
            }
            return bitmap;
        }
    }
}
