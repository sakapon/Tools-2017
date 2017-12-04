using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Reactive.Bindings;

namespace EpidemicSimulator
{
    public class MainViewModel
    {
        static readonly TimeSpan RenderingInterval = TimeSpan.FromSeconds(1 / 30.0);
        const int PopulationBarWidth = 800;

        public AppModel AppModel { get; } = new AppModel();

        public ReactiveProperty<InfectionModel> InfectionModel { get; } = new ReactiveProperty<InfectionModel>(mode: ReactivePropertyMode.DistinctUntilChanged);
        public ReadOnlyReactiveProperty<byte[]> PopulationImage { get; }
        public ReadOnlyReactiveProperty<PopulationSummary> PopulationSummary { get; }
        public ReadOnlyReactiveProperty<PopulationLayout> PopulationLayout { get; }

        IDisposable subscription;

        public MainViewModel()
        {
            PopulationImage = InfectionModel.Select(DataModelHelper.GetBitmapBinary).ToReadOnlyReactiveProperty();
            PopulationSummary = InfectionModel.Select(DataModelHelper.ToSummary).ToReadOnlyReactiveProperty();
            PopulationLayout = PopulationSummary.Select(ToLayout).ToReadOnlyReactiveProperty();

            AppModel.IsRunning
                .Where(b => b)
                .Subscribe(b =>
                {
                    subscription = Observable.Interval(RenderingInterval)
                        .Subscribe(_ => InfectionModel.Value = AppModel.CurrentInfection);
                });
            AppModel.IsRunning
                .Where(b => !b)
                .Subscribe(b =>
                {
                    subscription?.Dispose();
                    subscription = null;
                });
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
