using CompMath3.Math.Functions;

namespace CompMath3.Utils
{
    public static class FunctionFabric
    {
        private static readonly Function[] _functions =
        {
            new PolyFunc(),
            new CosFunc(),
            new ExpFunc(),
            new RootFunc(),
            new LinearAbsFunc(),
            new DivSingularity(),
            new InversSqrtSingularity(),
            new InternalSingularity()
        };

        public static (bool, Function?) GetFunctionById(int id)
        {
            if(id < 0 || id >= _functions.Length)
            {
                return (false, null);
            }

            return (true, _functions[id]);
        }

        public static List<string> GetFunctionStringList()
        {
            return _functions.Select((func, i) => $"{i}. {func.GetFunctionString()}").ToList();
        }
    }
}
