﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using Reactive.Bindings;

namespace EpidemicSimulator
{
    public class AppModel
    {
        public InitialSettings InitialSettings { get; } = new InitialSettings();
        public RealtimeSettings RealtimeSettings { get; } = new RealtimeSettings();

        public ReactiveProperty<bool> IsRunning { get; } = new ReactiveProperty<bool>(false);
        public InfectionModel CurrentInfection { get; set; }

        public AppModel()
        {
            CurrentInfection = InitializeInfection(InitialSettings.ToValue());

            IsRunning
                .Where(b => b)
                .Subscribe(_ => Simulate());
        }

        void Simulate()
        {
            CurrentInfection = InitializeInfection(InitialSettings.ToValue());

            while (IsRunning.Value)
            {
                var rs = RealtimeSettings.ToValue();
                Thread.Sleep(TimeSpan.FromSeconds(rs.ExecutionInterval));
                CurrentInfection = NextInfection(CurrentInfection, rs);
            }
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
                            var count_I = DataModel.GetNeighborPoints(size, new Point(i, j), rs.IsMapLooping)
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
