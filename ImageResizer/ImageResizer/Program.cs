using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer
{
    class Program
    {
        static double DefaultScale { get; } = Convert.ToDouble(ConfigurationManager.AppSettings["DefaultScale"]);

        static void Main(string[] args)
        {
        }
    }
}
