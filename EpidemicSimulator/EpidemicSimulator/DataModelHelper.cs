using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace EpidemicSimulator
{
    public static class DataModelHelper
    {
        public static Point[] GetNeighborPoints(Size size, Point p, bool isMapLooping)
            => isMapLooping ? GetNeighborPointsWithLooping(size, p) : GetNeighborPointsWithoutLooping(size, p);

        static Point[] GetNeighborPointsWithoutLooping(Size size, Point p)
        {
            var ps = new List<Point>();

            if (p.X > 0)
                ps.Add(new Point(p.X - 1, p.Y));
            if (p.X < size.Width - 1)
                ps.Add(new Point(p.X + 1, p.Y));
            if (p.Y > 0)
                ps.Add(new Point(p.X, p.Y - 1));
            if (p.Y < size.Height - 1)
                ps.Add(new Point(p.X, p.Y + 1));

            return ps.ToArray();
        }

        static Point[] GetNeighborPointsWithLooping(Size size, Point p)
        {
            return new[]
            {
                new Point(p.X > 0 ? p.X - 1 : size.Width - 1, p.Y),
                new Point(p.X < size.Width - 1 ? p.X + 1 : 0, p.Y),
                new Point(p.X, p.Y > 0 ? p.Y - 1 : size.Height - 1),
                new Point(p.X, p.Y < size.Height - 1 ? p.Y + 1 : 0),
            };
        }

        public static PopulationSummary ToSummary(InfectionModel model)
        {
            var width = model.InitialSettings.Size.Width;
            var height = model.InitialSettings.Size.Height;

            var summary = new PopulationSummary
            {
                Total = width * height,
            };

            for (var i = 0; i < width; i++)
                for (var j = 0; j < height; j++)
                {
                    switch (model.Statuses[i, j])
                    {
                        case InfectionStatus.Susceptible:
                            summary.Susceptible++;
                            break;
                        case InfectionStatus.Infectious:
                            summary.Infectious++;
                            break;
                        case InfectionStatus.Recovered:
                            summary.Recovered++;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }

            return summary;
        }

        static readonly Dictionary<InfectionStatus, Color> StatusColors = new Dictionary<InfectionStatus, Color>
        {
            { InfectionStatus.Susceptible, Color.FromArgb(153, 204, 255) },
            { InfectionStatus.Infectious, Color.FromArgb(255, 102, 0) },
            { InfectionStatus.Recovered, Color.FromArgb(51, 153, 0) },
        };

        public static byte[] GetBitmapBinary(InfectionModel model)
        {
            using (var bitmap = new Bitmap(model.InitialSettings.Size.Width, model.InitialSettings.Size.Height))
            {
                for (var i = 0; i < bitmap.Width; i++)
                    for (var j = 0; j < bitmap.Height; j++)
                        bitmap.SetPixel(i, j, StatusColors[model.Statuses[i, j]]);

                return ImageToBytes(bitmap, ImageFormat.Png);
            }
        }

        static byte[] ImageToBytes(Image image, ImageFormat format)
        {
            using (var memory = new MemoryStream())
            {
                image.Save(memory, format);
                return memory.ToArray();
            }
        }
    }
}
