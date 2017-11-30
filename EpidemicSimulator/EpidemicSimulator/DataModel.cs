using System;
using System.Collections.Generic;
using System.Drawing;
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
            Size = new Size(Width.Value, Height.Value),

            SusceptibleRatio = SusceptibleRatio.Value,
            InfectiousRatio = InfectiousRatio.Value,
        };
    }

    public struct VInitialSettings
    {
        public Size Size { get; set; }

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
        public static Point[] GetNeighborPoints(Size size, Point p, bool isMapLooping)
            => isMapLooping ? GetNeighborPointsWithLooping(size, p) : GetNeighborPointsWithoutLooping(size, p);

        static Point[] GetNeighborPointsWithoutLooping(Size size, Point p)
        {
            var ps = new List<Point>();

            if (p.X > 0)
                ps.Add(new Point(p.X - 1, p.Y));
            if (p.X < size.Width - 1)
                ps.Add(new Point(p.X + 1, p.Y));
            if (p.Y > 0)
                ps.Add(new Point(p.X, p.Y - 1));
            if (p.Y < size.Height - 1)
                ps.Add(new Point(p.X, p.Y + 1));

            return ps.ToArray();
        }

        static Point[] GetNeighborPointsWithLooping(Size size, Point p)
        {
            return new[]
            {
                new Point(p.X > 0 ? p.X - 1 : size.Width - 1, p.Y),
                new Point(p.X < size.Width - 1 ? p.X + 1 : 0, p.Y),
                new Point(p.X, p.Y > 0 ? p.Y - 1 : size.Height - 1),
                new Point(p.X, p.Y < size.Height - 1 ? p.Y + 1 : 0),
            };
        }

        public static PopulationSummary ToSummary(InfectionModel model)
        {
            var width = model.InitialSettings.Size.Width;
            var height = model.InitialSettings.Size.Height;

            var summary = new PopulationSummary
            {
                Total = width * height,
            };

            for (var i = 0; i < width; i++)
                for (var j = 0; j < height; j++)
                {
                    switch (model.Statuses[i, j])
                    {
                        case InfectionStatus.Susceptible:
                            summary.Susceptible++;
                            break;
                        case InfectionStatus.Infectious:
                            summary.Infectious++;
                            break;
                        case InfectionStatus.Recovered:
                            summary.Recovered++;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }

            return summary;
        }
    }
}
