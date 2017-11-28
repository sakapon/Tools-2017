using System;
using System.Collections.Generic;
using System.Linq;
using Reactive.Bindings;

namespace EpidemicSimulator
{
    public enum InfectionStatus
    {
        Susceptible,
        Infectious,
        Recovered,
    }

    public class InitialSettings
    {
        public ReactiveProperty<int> Width { get; } = new ReactiveProperty<int>(200);
        public ReactiveProperty<int> Height { get; } = new ReactiveProperty<int>(200);

        public ReactiveProperty<double> SusceptibleRatio { get; } = new ReactiveProperty<double>(0.7);
        public ReactiveProperty<double> InfectiousRatio { get; } = new ReactiveProperty<double>(0.1);

        public VInitialSettings ToValue() => new VInitialSettings
        {
            Width = Width.Value,
            Height = Height.Value,

            SusceptibleRatio = SusceptibleRatio.Value,
            InfectiousRatio = InfectiousRatio.Value,
        };
    }

    public struct VInitialSettings
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public double SusceptibleRatio { get; set; }
        public double InfectiousRatio { get; set; }
    }

    public class RealtimeSettings
    {
        public ReactiveProperty<double> InfectionRate { get; } = new ReactiveProperty<double>(0.1);
        public ReactiveProperty<double> RecoveryRate { get; } = new ReactiveProperty<double>(0.2);
        public ReactiveProperty<double> DeimmunizationRate { get; } = new ReactiveProperty<double>(0.3);

        public ReactiveProperty<double> ExecutionInterval { get; } = new ReactiveProperty<double>(0.05);
        public ReactiveProperty<bool> IsMapLooping { get; } = new ReactiveProperty<bool>(false);

        public VRealtimeSettings ToValue() => new VRealtimeSettings
        {
            InfectionRate = InfectionRate.Value,
            RecoveryRate = RecoveryRate.Value,
            DeimmunizationRate = DeimmunizationRate.Value,

            ExecutionInterval = ExecutionInterval.Value,
            IsMapLooping = IsMapLooping.Value,
        };
    }

    public struct VRealtimeSettings
    {
        public double InfectionRate { get; set; }
        public double RecoveryRate { get; set; }
        public double DeimmunizationRate { get; set; }

        public double ExecutionInterval { get; set; }
        public bool IsMapLooping { get; set; }
    }

    public struct InfectionModel
    {
        public VInitialSettings InitialSettings { get; set; }

        public int Turn { get; set; }
        public InfectionStatus[,] Statuses { get; set; }
    }

    public struct PopulationSummary
    {
        public int Total { get; set; }

        public int Susceptible { get; set; }
        public int Infectious { get; set; }
        public int Recovered { get; set; }
    }

    public static class DataModel
    {
    }
}
