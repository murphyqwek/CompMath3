using CompMath3.Math.Functions;
using PeterO.Numbers;

namespace CompMath3.Math.Methods
{
    public abstract class Rectangles : Method
    {
        public Rectangles(Function function, EDecimal a, EDecimal b, EDecimal eps, int k) : base(function, a, b, eps, k)
        {

        }

        protected override EDecimal CalculateIntegral(int n)
        {
            EDecimal h = CalculateH(n);
            return h * CalculateSumOfFunctionOnInterval(h, n);
        }   

        protected abstract EDecimal CalculateSumOfFunctionOnInterval(EDecimal h, int n);
    }
}
