using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace ImageResizer
{
    class Program
    {
        static readonly string[] ImageFileExtensions = new[] { ".jpg", ".png", ".gif" };

        static double DefaultScale { get; } = Convert.ToDouble(ConfigurationManager.AppSettings["DefaultScale"]);

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("ImageResizer [Dir-Path | File-Path] [-Scale 0.8 | -Width 600 | -Height 400]");
                return;
            }

            var path = args[0];
            var scale = DefaultScale;

            if (Directory.Exists(path))
                ScaleFiles(path, scale);
            else if (File.Exists(path))
                ScaleFile(path, scale);
            else
                Console.WriteLine($"{path} is not found.");
        }

        static void ScaleFiles(string dirPath, double scale)
        {
            var newDirPath = $"{dirPath}-re";
            if (Directory.Exists(newDirPath)) return;
            Directory.CreateDirectory(newDirPath);

            var filePaths = Directory.EnumerateFiles(dirPath)
                .Where(p => ImageFileExtensions.Any(e => p.EndsWith(e, StringComparison.InvariantCultureIgnoreCase)));
            foreach (var filePath in filePaths)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var newFilePath = Path.Combine(newDirPath, $"{fileName}.jpg");

                BitmapHelper.ScaleImageFile(filePath, newFilePath, scale);
            }
        }

        static void ScaleFile(string filePath, double scale)
        {
            var dirPath = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var newFilePath = Path.Combine(dirPath, $"{fileName}-re.jpg");

            BitmapHelper.ScaleImageFile(filePath, newFilePath, scale);
        }
    }
}
