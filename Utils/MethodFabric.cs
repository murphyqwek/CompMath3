using CompMath3.Math.Functions;
using CompMath3.Math.Methods;
using PeterO.Numbers;

namespace CompMath3.Utils
{
    public static class MethodFabric
    {
        private static readonly string[] _methods =
        {
            "Метод левых прямоугольников",
            "Метод правых прямоугольников",
            "Метод средних прямоугольников",
            "Метод трапеций",
            "Метод Сипсона",
        };

        public static (bool, Method?) GetMethodById(int id, Function func, EDecimal a, EDecimal b, EDecimal eps)
        {
            switch(id)
            {
                case 0:
                    return (true, new LeftRectrangles(func, a, b, eps));
                case 1:
                    return (true, new RightRectangles(func, a, b, eps));
                case 2:
                    return (true, new MidpointRectangle(func, a, b, eps));
                case 3:
                    return (true, new Trapezoid(func, a, b, eps));
                case 4:
                    return (true, new Simpson(func, a, b, eps));
                default:
                    return (false, null);
            }
        }

        public static List<string> GetMethodStringList()
        {
            return _methods.Select((method, i) => $"{i}. {method}").ToList();
        }
    }
}
