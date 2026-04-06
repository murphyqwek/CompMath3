using CompMath3.Math.Functions;
using PeterO.Numbers;

namespace CompMath3.Math.Methods
{
    public class Trapezoid : Method
    {
        public Trapezoid(Function function, EDecimal a, EDecimal b, EDecimal eps) : base(function, a, b, eps, 2)
        {
        }

        protected override EDecimal CalculateIntegral(int n)
        {
            EDecimal h = CalculateH(n);

            EDecimal y0 = _function.Calculate(_a);
            EDecimal yn = _function.Calculate(_b);

            EDecimal sum = 0;

            for(int i = 1; i <= n - 1; i++)
            {
                EDecimal xi = _a + h * i;

                sum += _function.Calculate(xi);
            }

            return h * ( (y0 + yn).Divide(2, _context) + sum);
        }

        public override string GetMethodName()
        {
            return "Метод трапеций";
        }
    }
}
