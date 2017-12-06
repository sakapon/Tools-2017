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

        public ReactiveProperty<double> SusceptibleRatioVar { get; }
        public ReactiveProperty<double> InfectiousRatioVar { get; }
        public ReadOnlyReactiveProperty<double> SusceptibleRatioMaxValue { get; }
        public ReadOnlyReactiveProperty<double> InfectiousRatioMaxValue { get; }

        public ReadOnlyReactiveProperty<byte[]> PopulationImage { get; }
        public ReadOnlyReactiveProperty<PopulationSummary> PopulationSummary { get; }
        public ReadOnlyReactiveProperty<PopulationLayout> PopulationLayout { get; }

        public MainViewModel()
        {
            SusceptibleRatioVar = new ReactiveProperty<double>(AppModel.InitialSettings.PopulationRatio.Value.SusceptibleRatio);
            InfectiousRatioVar = new ReactiveProperty<double>(AppModel.InitialSettings.PopulationRatio.Value.InfectiousRatio);

            SusceptibleRatioVar.Subscribe(s =>
            {
                var p0 = AppModel.InitialSettings.PopulationRatio.Value;
                var p = new PopulationRatio
                {
                    SusceptibleRatio = s,
                    InfectiousRatio = 1 - s - p0.RecoveredRatio,
                    RecoveredRatio = p0.RecoveredRatio,
                };

                AppModel.InitialSettings.PopulationRatio.Value = p;
                InfectiousRatioVar.Value = p.InfectiousRatio;
            });
            InfectiousRatioVar.Subscribe(i =>
            {
                var p0 = AppModel.InitialSettings.PopulationRatio.Value;
                var p = new PopulationRatio
                {
                    SusceptibleRatio = p0.SusceptibleRatio,
                    InfectiousRatio = i,
                    RecoveredRatio = 1 - p0.SusceptibleRatio - i,
                };

                AppModel.InitialSettings.PopulationRatio.Value = p;
            });

            SusceptibleRatioMaxValue = AppModel.InitialSettings.PopulationRatio
                .Select(p => 0.99 - p.RecoveredRatio)
                .ToReadOnlyReactiveProperty();
            InfectiousRatioMaxValue = AppModel.InitialSettings.PopulationRatio
                .Select(p => 1 - p.SusceptibleRatio)
                .ToReadOnlyReactiveProperty();

            PopulationImage = AppModel.InfectionSnapshot.Select(DataModelHelper.GetBitmapBinary).ToReadOnlyReactiveProperty();
            PopulationSummary = AppModel.InfectionSnapshot.Select(DataModelHelper.ToSummary).ToReadOnlyReactiveProperty();
            PopulationLayout = PopulationSummary.Select(ToLayout).ToReadOnlyReactiveProperty();
        }

        static PopulationLayout ToLayout(PopulationSummary s)
        {
            var width_i = (int)Math.Round(PopulationBarWidth * ((double)s.Infectious / s.Total), MidpointRounding.AwayFromZero);
            var width_r = (int)Math.Round(PopulationBarWidth * ((double)s.Recovered / s.Total), MidpointRounding.AwayFromZero);

            return new PopulationLayout
            {
                SusceptibleWidth = PopulationBarWidth - width_i - width_r,
                InfectiousWidth = width_i,
                RecoveredWidth = width_r,
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
