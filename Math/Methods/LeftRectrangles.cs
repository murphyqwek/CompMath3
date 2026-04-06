using CompMath3.Math.Functions;
using PeterO.Numbers;

namespace CompMath3.Math.Methods
{
    public class LeftRectrangles : Rectangles
    {
        public LeftRectrangles(Function function, EDecimal a, EDecimal b, EDecimal eps) : base(function, a, b, eps, 1)
        {
        }

        protected override EDecimal CalculateSumOfFunctionOnInterval(EDecimal h, int n)
        {
            EDecimal sum = 0;
            for (int i = 0; i <= n - 1; i++)
            {
                sum += _function.Calculate(_a + h.Multiply(i));
            }

            return sum;
        }

        public override string GetMethodName()
        {
            return "Метод левых прямоугольников";
        }
    }
}
