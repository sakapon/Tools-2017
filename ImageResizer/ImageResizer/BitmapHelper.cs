using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageResizer
{
    public static class BitmapHelper
    {
        public static void ScaleImageFile(string sourceFile, string destFile, double scale)
        {
            using (var source = Image.FromFile(sourceFile))
            using (var resized = ScaleImage(source, scale))
            {
                resized.Save(destFile, ImageFormat.Jpeg);
            }
        }

        public static Bitmap ScaleImage(Image source, double scale) =>
            BitmapUtility.ResizeImage(source, Round(scale * source.Width), Round(scale * source.Height));

        public static int Round(this double value) =>
            (int)Math.Round(value, MidpointRounding.AwayFromZero);
    }
}
