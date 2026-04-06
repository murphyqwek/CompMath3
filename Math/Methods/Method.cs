using CompMath3.Math.Functions;
using PeterO.Numbers;

namespace CompMath3.Math.Methods
{
    public record IntegralMethodResult(bool isSuccess, string errorMessage = "", EDecimal? result = null, int n = 0);

    public abstract class Method
    {
        protected readonly Function _function;
        protected readonly EDecimal _eps;
        protected readonly EDecimal _b;
        protected readonly EDecimal _a;

        protected readonly EContext _context = EContext.Decimal128;

        protected const int MAX_ITERATIONS = 1_000_000;

        protected readonly int _k;

        public Method(Function function, EDecimal a, EDecimal b, EDecimal eps, int k)
        {
            _function = function;
            _a = a;
            _b = b;
            _eps = eps;
            _k = k;
        }

        public IntegralMethodResult Calculate()
        {
            int n = 4;

            EDecimal integral = CalculateIntegral(n);

            while (n <= MAX_ITERATIONS)
            {
                n *= 2;

                EDecimal halfStepIntegral = CalculateIntegral(n);

                EDecimal R = CalculateR(halfStepIntegral, integral, _k);

                if (R.CompareTo(_eps) < 0)
                {
                    return new IntegralMethodResult(true, "", halfStepIntegral, n);
                }

                integral = halfStepIntegral;
            }

            return new IntegralMethodResult(false, "Метод не смог найти решение с заданной точностью за максимальное количество шагов. Возможно, функция на интервале слишком быстро растёт. Выберите другой интервал");
        }

        protected abstract EDecimal CalculateIntegral(int n);

        protected EDecimal CalculateH(int n)
        {
            return (_b - _a).Divide(n, _context);
        }

        protected EDecimal CalculateR(EDecimal IntergralHalf, EDecimal Integral, int k)
        {
            EDecimal numerator = (IntergralHalf - Integral).Abs();

            EDecimal denominator = EDecimal.FromDouble(System.Math.Pow(2, k) - 1);

            return numerator.Divide(denominator, _context);
        }

        public abstract string GetMethodName();
    }
}
