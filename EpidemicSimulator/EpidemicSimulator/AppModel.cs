using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Reactive.Bindings;

namespace EpidemicSimulator
{
    public class AppModel
    {
        static readonly TimeSpan SnapshotInterval = TimeSpan.FromSeconds(1 / 30.0);

        public InitialSettings InitialSettings { get; } = new InitialSettings();
        public RealtimeSettings RealtimeSettings { get; } = new RealtimeSettings();

        InfectionModel _CurrentInfection;
        public ReactiveProperty<InfectionModel> InfectionSnapshot { get; } = new ReactiveProperty<InfectionModel>();

        public ReactiveProperty<bool> IsRunning { get; } = new ReactiveProperty<bool>(false);

        public AppModel()
        {
            _CurrentInfection = InitializeInfection(InitialSettings.ToValue());
            InfectionSnapshot.Value = _CurrentInfection;

            CreateObserver(InitialSettings)
                .Throttle(TimeSpan.FromSeconds(0.1))
                .Subscribe(_ =>
                {
                    _CurrentInfection = InitializeInfection(InitialSettings.ToValue());
                    InfectionSnapshot.Value = _CurrentInfection;
                });

            IsRunning
                .Where(b => b)
                .ObserveOn(TaskPoolScheduler.Default)
                .Subscribe(_ => Simulate());
        }

        void Simulate()
        {
            var subscription = Observable.Interval(SnapshotInterval)
                .Subscribe(_ => InfectionSnapshot.Value = _CurrentInfection);

            if (_CurrentInfection.Turn > 0)
                _CurrentInfection = InitializeInfection(InitialSettings.ToValue());

            while (IsRunning.Value)
            {
                var rs = RealtimeSettings.ToValue();
                Thread.Sleep(TimeSpan.FromSeconds(rs.ExecutionInterval));
                _CurrentInfection = NextInfection(_CurrentInfection, rs);
            }

            subscription.Dispose();
        }

        static Subject<Unit> CreateObserver(InitialSettings s)
        {
            var subject = new Subject<Unit>();

            s.Width.Merge(s.Height)
                .Select(_ => Unit.Default)
                .Subscribe(subject);
            s.SusceptibleRatio.Merge(s.InfectiousRatio)
                .Select(_ => Unit.Default)
                .Subscribe(subject);

            return subject;
        }

        static InfectionModel InitializeInfection(VInitialSettings s)
        {
            var statuses = new InfectionStatus[s.Size.Width, s.Size.Height];

            var probs = new Dictionary<InfectionStatus, double>
            {
                { InfectionStatus.Susceptible, s.SusceptibleRatio },
                { InfectionStatus.Infectious, s.InfectiousRatio },
                { InfectionStatus.Recovered, 1.0 - s.SusceptibleRatio - s.InfectiousRatio },
            };

            for (var i = 0; i < s.Size.Width; i++)
                for (var j = 0; j < s.Size.Height; j++)
                    statuses[i, j] = probs.GetRandomElement();

            return new InfectionModel
            {
                InitialSettings = s,
                Turn = 0,
                Statuses = statuses,
            };
        }

        // S' = θR - βSI
        // I' = βSI - γI
        // R' = γI - θR
        static InfectionModel NextInfection(InfectionModel model, VRealtimeSettings rs)
        {
            var β = rs.InfectionRate;
            var γ = rs.RecoveryRate;
            var θ = rs.DeimmunizationRate;

            var size = model.InitialSettings.Size;
            var statuses = new InfectionStatus[size.Width, size.Height];

            for (var i = 0; i < size.Width; i++)
                for (var j = 0; j < size.Height; j++)
                {
                    switch (model.Statuses[i, j])
                    {
                        case InfectionStatus.Susceptible:
                            var count_I = DataModelHelper.GetNeighborPoints(size, new Point(i, j), rs.IsMapLooping)
                                .Select(p => model.Statuses[p.X, p.Y])
                                .Count(x => x == InfectionStatus.Infectious);
                            statuses[i, j] = count_I == 0 ? InfectionStatus.Susceptible : RandomHelper.GetRandomElement(1 - Math.Pow(1 - β, count_I), InfectionStatus.Infectious, InfectionStatus.Susceptible);
                            break;
                        case InfectionStatus.Infectious:
                            statuses[i, j] = RandomHelper.GetRandomElement(γ, InfectionStatus.Recovered, InfectionStatus.Infectious);
                            break;
                        case InfectionStatus.Recovered:
                            statuses[i, j] = RandomHelper.GetRandomElement(θ, InfectionStatus.Susceptible, InfectionStatus.Recovered);
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }

            return new InfectionModel
            {
                InitialSettings = model.InitialSettings,
                Turn = model.Turn + 1,
                Statuses = statuses,
            };
        }
    }
}
