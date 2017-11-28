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
        public int Width;
        public int Height;

        public double SusceptibleRatio;
        public double InfectiousRatio;
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
        public double InfectionRate;
        public double RecoveryRate;
        public double DeimmunizationRate;

        public double ExecutionInterval;
        public bool IsMapLooping;
    }

    public struct PopulationSummary
    {
        public int Total;

        public int Susceptible;
        public int Infectious;
        public int Recovered;
    }

    public static class DataModel
    {
    }
}
