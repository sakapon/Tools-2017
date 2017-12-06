using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EpidemicSimulator
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        const double SliderWidthOffset = 11;
        const double SliderWidth = 280 - SliderWidthOffset;

        public static readonly Func<bool, bool> InverseBoolean = x => !x;
        public static readonly Func<double, double> ToSusceptibleRatioWidth = x => SliderWidth * x + SliderWidthOffset;
        public static readonly Func<double, double> ToInfectiousRatioWidth = x => SliderWidth * (x - 0.01) + SliderWidthOffset;

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
