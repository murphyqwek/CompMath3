using CompMath3.Math.Functions;
using PeterO.Numbers;
using System.Collections.Generic;
using System.Linq;

namespace CompMath3.Math.Discontinuity
{
    public class DiscontinuityPoint
    {
        public EDecimal X { get; set; }
        public bool IsConvergent { get; set; }
    }

    public static class Utils
    {
        private static readonly int MaxRecursionDepth = 40;

        public static List<DiscontinuityPoint> FindDiscontinuities(Function f, EDecimal a, EDecimal b)
        {
            var points = new List<DiscontinuityPoint>();

            EDecimal range = (b - a).Abs();
            int steps = (int)System.Math.Clamp(range.ToDouble() * 200, 1000, 2_000_000);
            EDecimal step = range / EDecimal.FromInt32(steps);

            for (int i = 0; i < steps; i++)
            {
                EDecimal x1 = a + step * i;
                EDecimal x2 = EDecimal.Min(a + step * (i + 1), b);

                Scan(f, x1, x2, 0, points, a, b);
            }

            return points
                .GroupBy(p => System.Math.Round(p.X.ToDouble(), 9))
                .Select(g => g.First())
                .OrderBy(p => p.X)
                .ToList();
        }

        private static void Scan(Function f, EDecimal x1, EDecimal x2, int depth, List<DiscontinuityPoint> found, EDecimal globalA, EDecimal globalB)
        {
            EDecimal y1 = TryEval(f, x1);
            EDecimal y2 = TryEval(f, x2);
            EDecimal mid = (x1 + x2) / EDecimal.FromInt32(2);
            EDecimal yMid = TryEval(f, mid);

            bool hasInfinity = y1.IsInfinity() || y2.IsInfinity() || yMid.IsInfinity();
            if (!hasInfinity) return;

            if (depth >= MaxRecursionDepth || (x2 - x1).Abs().CompareTo(EDecimal.FromDouble(1e-14)) < 0)
            {
                EDecimal x = Refine(f, x1, x2);

                if (!found.Any(p => (p.X - x).Abs().CompareTo(EDecimal.FromDouble(1e-9)) < 0))
                {
                    found.Add(new DiscontinuityPoint
                    {
                        X = x,
                        IsConvergent = CheckConvergence(f, x, globalA, globalB)
                    });
                }
            }
            else
            {
                Scan(f, x1, mid, depth + 1, found, globalA, globalB);
                Scan(f, mid, x2, depth + 1, found, globalA, globalB);
            }
        }

        private static EDecimal Refine(Function f, EDecimal low, EDecimal high)
        {
            for (int i = 0; i < 60; i++)
            {
                EDecimal m1 = low + (high - low) * EDecimal.FromDouble(0.382);
                EDecimal m2 = low + (high - low) * EDecimal.FromDouble(0.618);

                if (SafeAbs(f, m1).CompareTo(SafeAbs(f, m2)) > 0)
                    high = m2;
                else
                    low = m1;
            }

            return (low + high) / EDecimal.FromInt32(2);
        }

        private static bool CheckConvergence(Function f, EDecimal x, EDecimal a, EDecimal b)
        {
            bool leftConverges = (x - a).Abs().CompareTo(EDecimal.FromDouble(1e-6)) < 0 || CheckSide(f, x, -1, x - a);
            bool rightConverges = (b - x).Abs().CompareTo(EDecimal.FromDouble(1e-6)) < 0 || CheckSide(f, x, 1, b - x);

            return leftConverges && rightConverges;
        }

        private static bool CheckSide(Function f, EDecimal x, int dir, EDecimal maxDist)
        {
            EDecimal dist = maxDist.CompareTo(EDecimal.FromDouble(0.1)) > 0 ? EDecimal.FromDouble(0.1) : maxDist;
            EDecimal sum = EDecimal.Zero;
            EDecimal prevI = EDecimal.Zero;

            for (int k = 0; k < 30; k++)
            {
                EDecimal pow1 = EDecimal.FromDouble(System.Math.Pow(2, k));
                EDecimal pow2 = EDecimal.FromDouble(System.Math.Pow(2, k + 1));

                EDecimal a_k, b_k;
                if (dir == 1)
                {
                    a_k = x + dist / pow2;
                    b_k = x + dist / pow1;
                }
                else
                {
                    a_k = x - dist / pow1;
                    b_k = x - dist / pow2;
                }

                EDecimal currentI = Integrate(f, a_k, b_k, 200);

                if (currentI.IsInfinity() || currentI.IsNaN()) return false;

                if (k > 5 && currentI.CompareTo(prevI * EDecimal.FromDouble(0.95)) >= 0) return false;

                sum += currentI;
                if (sum.CompareTo(EDecimal.FromDouble(10000)) > 0) return false;

                prevI = currentI;
            }

            return true;
        }

        private static EDecimal Integrate(Function f, EDecimal a, EDecimal b, int n)
        {
            if (a.CompareTo(b) >= 0) return EDecimal.Zero;

            EDecimal h = (b - a).Abs() / EDecimal.FromInt32(n);
            EDecimal sum = EDecimal.Zero;

            for (int i = 0; i < n; i++)
            {
                EDecimal m = a + h * (EDecimal.FromInt32(i) + EDecimal.FromDouble(0.5));
                var y = SafeAbs(f, m);

                if (y.IsInfinity())
                    return EDecimal.PositiveInfinity;

                sum += y;
            }

            return sum * h;
        }

        private static EDecimal SafeAbs(Function f, EDecimal x)
        {
            try
            {
                var v = f.Calculate(x);
                return (v.IsNaN() || v.IsInfinity()) ? EDecimal.PositiveInfinity : v.Abs();
            }
            catch
            {
                return EDecimal.PositiveInfinity;
            }
        }

        private static EDecimal TryEval(Function f, EDecimal x)
        {
            try
            {
                var v = f.Calculate(x);
                return (v.IsNaN() || v.IsInfinity()) ? EDecimal.PositiveInfinity : v;
            }
            catch
            {
                return EDecimal.PositiveInfinity;
            }
        }

        public static bool IsIntegralConvergent(List<DiscontinuityPoint> points)
        {
            if (points == null || points.Count == 0) return true;
            return points.All(p => p.IsConvergent);
        }
    }
}