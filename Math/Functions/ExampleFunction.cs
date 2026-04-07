using PeterO.Numbers;


namespace CompMath3.Math.Functions
{
    public class PolyFunc : Function
    {
        public override EDecimal Calculate(EDecimal x) 
        { 
            return x.Multiply(x).Add(x.Multiply(EDecimal.FromInt32(3))).Subtract(EDecimal.FromInt32(5));
        }

        public override string GetFunctionString() => "f(x) = x^2 + 3x - 5";
    }

    public class CosFunc : Function
    {
        public override EDecimal Calculate(EDecimal x)
        {
            return EDecimal.FromDouble(System.Math.Cos(x.ToDouble()));
        }

        public override string GetFunctionString() => "f(x) = cos(x)";
    }

    public class ExpFunc : Function
    {
        public override EDecimal Calculate(EDecimal x)
        {
            return EDecimal.FromDouble(System.Math.Exp(x.ToDouble()));
        }

        public override string GetFunctionString() => "f(x) = e^x";
    }

    public class RootFunc : Function
    {
        public override EDecimal Calculate(EDecimal x)
        {
            return EDecimal.FromDouble(System.Math.Sqrt(x.Multiply(x).Add(1).ToDouble()));
        }

        public override string GetFunctionString() => "f(x) = sqrt(x^2 + 1)";
    }

    public class LinearAbsFunc : Function
    {
        public override EDecimal Calculate(EDecimal x)
        {
            return x.Abs().Add(2);
        }

        public override string GetFunctionString() => "f(x) = |x| + 2";
    }

    public class DivSingularity : Function
    {
        public override EDecimal Calculate(EDecimal x)
        {
            EDecimal x2 = x.Multiply(x);
            return EDecimal.One.Divide(x2, EContext.Decimal128);
        }

        public override string GetFunctionString() => "f(x) = 1 / x^2";
    }

    public class InversSqrtSingularity : Function
    {
        public override EDecimal Calculate(EDecimal x)
        {
            var d = x.Abs().ToDouble();

            return EDecimal.FromDouble(1 / System.Math.Sqrt(d));
        }

        public override string GetFunctionString() => "f(x) = 1 / sqrt(|x|)";
    }

    public class InternalSingularity : Function
    {
        public override EDecimal Calculate(EDecimal x)
        {
            double diff = (x - EDecimal.One).Abs().ToDouble();
            return EDecimal.FromInt32(1).Divide(x, EContext.Decimal128);
        }

        public override string GetFunctionString() => "f(x) = 1 / x";
    }
}
