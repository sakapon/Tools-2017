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

        static CommandArgs ParseArgs(string[] args)
        {
            var cargs = new CommandArgs();

            if (args.TryGetItem(0, out var a_help) &&
                (a_help.Contains("?") || string.Equals(a_help, "-Help", StringComparison.InvariantCultureIgnoreCase)))
            {
                cargs.Help = true;
                return cargs;
            }

            if (args.TryGetItem(0, out var a_path) && !a_path.StartsWith("-"))
                cargs.Path = a_path;

            if (TryGetDouble(args, "Scale", out var scale))
                cargs.Scale = scale;
            if (TryGetInt32(args, "Width", out var width))
                cargs.Width = width;
            if (TryGetInt32(args, "Height", out var height))
                cargs.Height = height;

            return cargs;
        }

        static bool TryGetInt32(string[] args, string command, out int value)
        {
            value = -1;
            var index = args.FirstIndex(a => string.Equals(a, "-" + command, StringComparison.InvariantCultureIgnoreCase));
            return index.HasValue &&
                args.TryGetItem(index.Value + 1, out var arg) &&
                int.TryParse(arg, out value);
        }

        static bool TryGetDouble(string[] args, string command, out double value)
        {
            value = -1;
            var index = args.FirstIndex(a => string.Equals(a, "-" + command, StringComparison.InvariantCultureIgnoreCase));
            return index.HasValue &&
                args.TryGetItem(index.Value + 1, out var arg) &&
                double.TryParse(arg, out value);
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

    public class CommandArgs
    {
        public bool Help { get; set; }
        public string Path { get; set; }
        public double? Scale { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
    }
}
