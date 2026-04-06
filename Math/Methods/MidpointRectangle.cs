using CompMath3.Math.Functions;
using PeterO.Numbers;

namespace CompMath3.Math.Methods
{
    public class MidpointRectangle : Rectangles
    {
        public MidpointRectangle(Function function, EDecimal a, EDecimal b, EDecimal eps) : base(function, a, b, eps, 2)
        {
        }

        protected override EDecimal CalculateSumOfFunctionOnInterval(EDecimal h, int n)
        {
            EDecimal sum = 0;
            for (int i = 0; i <= n - 1; i++)
            {
                var xi = _a + h.Multiply(EDecimal.FromInt32(i) + EDecimal.FromString("0.5"));
                sum += _function.Calculate(xi);
            }

            return sum;
        }

        public override string GetMethodName()
        {
            return "Метод средних прямоугольников";
        }
    }
}
