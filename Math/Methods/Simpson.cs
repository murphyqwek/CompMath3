using CompMath3.Math.Functions;
using PeterO.Numbers;

namespace CompMath3.Math.Methods
{
    public class Simpson : Method
    {
        public Simpson(Function function, EDecimal a, EDecimal b, EDecimal eps) : base(function, a, b, eps, 4)
        {
        }

        protected override EDecimal CalculateIntegral(int n)
        {
            EDecimal h = CalculateH(n);

            EDecimal y0 = _function.Calculate(_a);
            EDecimal yn = _function.Calculate(_b);

            EDecimal yEven = 0;
            EDecimal yOdd = 0;

            for(int i = 1; i <= n - 1; i += 2)
            {
                yOdd += _function.Calculate(_a + i * h);
            }

            for (int i = 2; i <= n - 2; i += 2)
            {
                yEven += _function.Calculate(_a + i * h);
            }

            h = h.Divide(3, _context);

            return h * (y0 + 4 * yOdd + 2 * yEven + yn);
        }

        public override string GetMethodName()
        {
            return "Метод Симпсона";
        }
    }
}
