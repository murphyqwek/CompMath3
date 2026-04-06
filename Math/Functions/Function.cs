using PeterO.Numbers;

namespace CompMath3.Math.Functions
{
    public abstract class Function
    {
        public abstract EDecimal Calculate(EDecimal value);

        public abstract string GetFunctionString();
    }
}
