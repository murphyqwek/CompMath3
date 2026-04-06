using CompMath3.Math.Functions;
using PeterO.Numbers;

namespace CompMath3.Math.Discontinuity
{
    public enum DiscontinuityType { Removable, Infinite }

    public class DiscontinuityPoint
    {
        public EDecimal X { get; set; }
        public DiscontinuityType Type { get; set; }
        public bool IsConvergent { get; set; }
    }

    public static class Utils
    {
        private static readonly EDecimal ConvergenceThreshold = EDecimal.FromDouble(0.005);
        private static readonly int MaxRecursionDepth = 40;

        public static List<DiscontinuityPoint> FindDiscontinuities(Function f, EDecimal a, EDecimal b)
        {
            var points = new List<DiscontinuityPoint>();
            EDecimal range = (b - a).Abs();

            double rangeD = range.ToDouble();
            int initialSteps = (int)System.Math.Clamp(rangeD * 100, 1000, 2_000_000);

            EDecimal step = range / EDecimal.FromInt32(initialSteps);

            for (int i = 0; i < initialSteps; i++)
            {
                EDecimal x1 = a + (step * EDecimal.FromInt32(i));
                EDecimal x2 = EDecimal.Min(a + (step * EDecimal.FromInt32(i + 1)), b);

                ScanSegmentRecursive(f, x1, x2, 0, points, a, b);
            }

            return points
                .GroupBy(p => System.Math.Round(p.X.ToDouble(), 9))
                .Select(g => g.First())
                .OrderBy(p => p.X)
                .ToList();
        }

        private static void ScanSegmentRecursive(Function f, EDecimal x1, EDecimal x2, int depth, List<DiscontinuityPoint> found, EDecimal a, EDecimal b)
        {
            EDecimal y1 = TryEval(f, x1).Abs();
            EDecimal y2 = TryEval(f, x2).Abs();
            EDecimal mid = (x1 + x2) / EDecimal.FromInt32(2);
            EDecimal yMid = TryEval(f, mid).Abs();

            bool suspicious = false;

            if (y1.IsInfinity() || y2.IsInfinity() || yMid.IsInfinity()) suspicious = true;

            else if (yMid.CompareTo(y1) > 0 && yMid.CompareTo(y2) > 0)
            {
                if (depth > 5 || yMid.CompareTo((y1 + y2) * EDecimal.FromDouble(1.1)) > 0) suspicious = true;
            }

            else if (y1.CompareTo(100) > 0 && y2.CompareTo(100) > 0 && TryEval(f, x1).Sign != TryEval(f, x2).Sign)
            {
                suspicious = true;
            }

            if (suspicious)
            {
                if (depth >= MaxRecursionDepth || (x2 - x1).Abs().CompareTo(EDecimal.FromDouble(1e-15)) < 0)
                {
                    EDecimal preciseX = RefinePoint(f, x1, x2);
                    if (!found.Any(p => (p.X - preciseX).Abs().CompareTo(EDecimal.FromDouble(1e-9)) < 0))
                    {
                        found.Add(AnalyzePoint(f, preciseX, a, b));
                    }
                }
                else
                {
                    ScanSegmentRecursive(f, x1, mid, depth + 1, found, a, b);
                    ScanSegmentRecursive(f, mid, x2, depth + 1, found, a, b);
                }
            }
        }

        private static EDecimal RefinePoint(Function f, EDecimal low, EDecimal high)
        {
            for (int i = 0; i < 70; i++)
            {
                EDecimal m1 = low + (high - low) * EDecimal.FromDouble(0.382);
                EDecimal m2 = low + (high - low) * EDecimal.FromDouble(0.618);

                if (TryEval(f, m1).Abs().CompareTo(TryEval(f, m2).Abs()) > 0 ) high = m2;
                else low = m1;
            }
            return (low + high) / EDecimal.FromInt32(2);
        }

        private static DiscontinuityPoint AnalyzePoint(Function f, EDecimal x, EDecimal a, EDecimal b)
        {
            var point = new DiscontinuityPoint { X = x };
            EDecimal eps = EDecimal.FromDouble(1e-11);

            EDecimal vL = TryEval(f, x - eps).Abs();
            EDecimal vR = TryEval(f, x + eps).Abs();

            if (vL.CompareTo(5000) < 0 && vR.CompareTo(5000) < 0 && !vL.IsInfinity() && !vR.IsInfinity())
            {
                point.Type = DiscontinuityType.Removable;
                point.IsConvergent = true;
            }
            else
            {
                point.Type = DiscontinuityType.Infinite;
                point.IsConvergent = CheckConvergence(f, x, a, b);
            }
            return point;
        }

        private static bool CheckConvergence(Function f, EDecimal x, EDecimal a, EDecimal b)
        {
            EDecimal radius = EDecimal.FromDouble(0.01);
            EDecimal eps1 = EDecimal.FromDouble(1e-5);
            EDecimal eps2 = EDecimal.FromDouble(1e-8);

            Func<EDecimal, EDecimal, bool> IsSideConvergent = (start, end) =>
            {
                if ((start - end).Abs().CompareTo(EDecimal.FromDouble(1e-12)) < 0) return true;

                EDecimal i1 = SimpleIntegrate(f, start, end, eps1, x);
                EDecimal i2 = SimpleIntegrate(f, start, end, eps2, x);

                if (i1.IsInfinity() || i2.IsInfinity()) return false;
                return (i1 - i2).Abs().CompareTo(ConvergenceThreshold) < 0;
            };

            bool left = (x.CompareTo(a) <= 0) || IsSideConvergent(EDecimal.Max(a, x - radius), x);
            bool right = (x.CompareTo(b) >= 0) || IsSideConvergent(x, EDecimal.Min(b, x + radius));

            return left && right;
        }

        private static EDecimal SimpleIntegrate(Function f, EDecimal a, EDecimal b, EDecimal eps, EDecimal targetX)
        {
            EDecimal start = a;
            EDecimal end = b;

            if (a.Equals(targetX)) start = a + eps;
            if (b.Equals(targetX)) end = b - eps;

            if (start.CompareTo(end) >= 0 && a.CompareTo(b) < 0) return EDecimal.Zero;

            int n = 300;
            EDecimal h = (end - start).Abs() / EDecimal.FromInt32(n);
            EDecimal sum = EDecimal.Zero;

            for (int i = 0; i < n; i++)
            {
                EDecimal m = start + h * (EDecimal.FromInt32(i) + EDecimal.FromDouble(0.5));
                EDecimal y = TryEval(f, m).Abs();
                if (y.IsInfinity()) return EDecimal.PositiveInfinity;
                sum += y;
            }
            return sum * h;
        }

        private static EDecimal TryEval(Function f, EDecimal x)
        {
            try
            {
                EDecimal val = f.Calculate(x);
                return (val.IsNaN() || val.IsInfinity()) ? EDecimal.PositiveInfinity : val;
            }
            catch { return EDecimal.PositiveInfinity; }
        }

        public static bool IsIntegralConvergent(List<DiscontinuityPoint> points) => points.All(p => p.IsConvergent);
    }
}