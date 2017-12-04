using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Reactive.Bindings;

namespace EpidemicSimulator
{
    public class MainViewModel
    {
        const int PopulationBarWidth = 800;

        public AppModel AppModel { get; } = new AppModel();

        public ReadOnlyReactiveProperty<byte[]> PopulationImage { get; }
        public ReadOnlyReactiveProperty<PopulationSummary> PopulationSummary { get; }
        public ReadOnlyReactiveProperty<PopulationLayout> PopulationLayout { get; }

        public MainViewModel()
        {
            PopulationImage = AppModel.InfectionSnapshot.Select(DataModelHelper.GetBitmapBinary).ToReadOnlyReactiveProperty();
            PopulationSummary = AppModel.InfectionSnapshot.Select(DataModelHelper.ToSummary).ToReadOnlyReactiveProperty();
            PopulationLayout = PopulationSummary.Select(ToLayout).ToReadOnlyReactiveProperty();
        }

        static PopulationLayout ToLayout(PopulationSummary s)
        {
            var width_s = (int)Math.Round(PopulationBarWidth * ((double)s.Susceptible / s.Total), MidpointRounding.AwayFromZero);
            var width_i = (int)Math.Round(PopulationBarWidth * ((double)s.Infectious / s.Total), MidpointRounding.AwayFromZero);

            return new PopulationLayout
            {
                SusceptibleWidth = width_s,
                InfectiousWidth = width_i,
                RecoveredWidth = PopulationBarWidth - width_s - width_i,
            };
        }
    }

    public struct PopulationLayout
    {
        public int SusceptibleWidth { get; set; }
        public int InfectiousWidth { get; set; }
        public int RecoveredWidth { get; set; }
    }
}
