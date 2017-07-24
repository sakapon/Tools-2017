using System;
using System.Drawing;

namespace ImageResizer
{
    public static class BitmapHelper
    {
        public static Bitmap ScaleImage(Image source, double scale) =>
            BitmapUtility.ResizeImage(source, Round(scale * source.Width), Round(scale * source.Height));

        public static int Round(this double value) =>
            (int)Math.Round(value, MidpointRounding.AwayFromZero);
    }
}
