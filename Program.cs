using CompMath3.Math.Discontinuity;
using CompMath3.Math.Functions;
using CompMath3.Math.Methods;
using CompMath3.Utils;
using PeterO.Numbers;

namespace CompMath3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CancelKeyPress += (sender, e) => Exit();

            RunMainLoop();
        }

        static void RunMainLoop()
        {
            while (true)
            {
                var func = SelectFunction();
                if (func == null) break;

                RunIntervalLoop(func);
            }
            Exit();
        }

        static Function? SelectFunction()
        {
            List<string> funcs = FunctionFabric.GetFunctionStringList();
            int lastId = funcs.Count - 1;

            while (true)
            {
                Console.Clear();
                ColorPrint.Print("ВЫБОР ФУНКЦИИ\n", ConsoleColor.Cyan);
                funcs.ForEach(Console.WriteLine);
                Console.WriteLine("\nВведите номер функции или 'exit' для выхода:");

                string? input = Console.ReadLine();
                if (input?.ToLower() == "exit") return null;

                if (int.TryParse(input, out int id) && id >= 0 && id <= lastId)
                {
                    var result = FunctionFabric.GetFunctionById(id);
                    if (result.Item1) return result.Item2;
                }

                ColorPrint.PrintError($"Неверный ввод. Введите число от 0 до {lastId}");
                AwaitKey();
            }
        }

        static void RunIntervalLoop(Function func)
        {
            while (true)
            {
                Console.Clear();
                ColorPrint.Print($"Функция: {func.GetFunctionString()}\n", ConsoleColor.Yellow);

                string? strA = ReadInput("Введите левый край (a): ");
                if (strA == null) return;

                if (!TryParseEDecimal(strA, out EDecimal a))
                {
                    ColorPrint.PrintError("Неверный формат числа a");
                    AwaitKey();
                    continue;
                }

                string? strB = ReadInput("Введите правый край (b): ");
                if (strB == null) return;

                if (!TryParseEDecimal(strB, out EDecimal b))
                {
                    ColorPrint.PrintError("Неверный формат числа b");
                    AwaitKey();
                    continue;
                }

                if (a.CompareTo(b) > 0)
                {
                    ColorPrint.PrintError("Неверно задан инетрвал: b должно быть больше a");
                    AwaitKey();
                    continue;
                }

                var intervalData = GetIntervalsToIntegrate(a, b, func);
                if (!intervalData.success)
                {
                    ColorPrint.PrintError(intervalData.message);
                    AwaitKey();
                    continue;
                }

                ColorPrint.Print($"\n{intervalData.message}", ConsoleColor.Green);
                AwaitKey();

                if (RunEpsilonLoop(intervalData.intervals, func)) return;
            }
        }

        static bool RunEpsilonLoop(List<(EDecimal, EDecimal)> intervals, Function func)
        {
            while (true)
            {
                Console.Clear();
                PrintHeader(func, intervals);

                string? strEps = ReadInput("Укажите погрешность (eps): ");
                if (strEps == null) return false;

                if (!TryParseEDecimal(strEps, out EDecimal eps) || eps.CompareTo(EDecimal.Zero) <= 0)
                {
                    ColorPrint.PrintError("Eps должен быть положительным числом.");
                    AwaitKey();
                    continue;
                }

                if (RunMethodLoop(intervals, func, eps)) return true;
            }
        }

        static bool RunMethodLoop(List<(EDecimal, EDecimal)> intervals, Function func, EDecimal eps)
        {
            var methods = MethodFabric.GetMethodStringList();
            int lastId = methods.Count - 1;

            while (true)
            {
                Console.Clear();
                PrintHeader(func, intervals, eps);

                Console.WriteLine("Выберите метод решения:");
                methods.ForEach(Console.WriteLine);

                string? choiceStr = ReadInput("\nНомер метода: ");
                if (choiceStr == null) return false;

                if (int.TryParse(choiceStr, out int id) && id >= 0 && id <= lastId)
                {
                    Func<EDecimal, EDecimal, Method> methodFactory = (a, b) =>
                        MethodFabric.GetMethodById(id, func, a, b, eps).Item2!;

                    PrintResult(intervals, func, eps, methodFactory);
                    AwaitKey();
                    return true;
                }

                ColorPrint.PrintError("Неверный выбор метода.");
                AwaitKey();
            }
        }

        static void PrintResult(List<(EDecimal, EDecimal)> intervals,
                                Function func,
                                EDecimal eps,
                                Func<EDecimal, EDecimal, Method> methodFactory)
        {
            Console.Clear();
            ColorPrint.Print("Вычисление\n", ConsoleColor.Cyan);

            EDecimal totalIntegral = EDecimal.Zero;
            bool success = true;

            string errorMessage = "";

            string headerFormat = "{0,-21} | {1,-20} | {2,-15}";
            string rowFormat = "[{0,8:F4}; {1,9:F4}] | {2,20:F5} | {3,15}";

            Console.WriteLine(headerFormat, "Интервал", "Значение", "Разбиение(n)");

            foreach (var (a, b) in intervals)
            {
                var method = methodFactory(a, b);
                var methodResult = method.Calculate();

                if (!methodResult.isSuccess)
                {
                    success = false;
                    errorMessage = methodResult.errorMessage;
                    break;
                }

                totalIntegral = totalIntegral.Add(methodResult.result);

                Console.WriteLine(rowFormat,
                    a.ToDouble(), b.ToDouble(), methodResult.result.ToDouble(), methodResult.n);
            }

            if (success)
            {
                ColorPrint.Print($"\nИтоговое значение интеграла: {FormatEDecimal(totalIntegral)}\n", ConsoleColor.Green);
            }
            else
            {
                Console.Clear();
                ColorPrint.Print("Вычисление\n", ConsoleColor.Cyan);
                ColorPrint.PrintError(errorMessage);
            }
        }

        static string? ReadInput(string prompt)
        {
            ColorPrint.Print(prompt, ConsoleColor.Gray);
            string? input = Console.ReadLine();
            return (input?.ToLower() == "exit") ? null : input;
        }

        static bool TryParseEDecimal(string input, out EDecimal result)
        {
            try
            {
                result = EDecimal.FromString(input.Replace(',', '.').Trim());
                return true;
            }
            catch
            {
                result = EDecimal.Zero;
                return false;
            }
        }

        static void PrintHeader(Function func, List<(EDecimal, EDecimal)> intervals, EDecimal? eps = null)
        {
            ColorPrint.Print($"Выбранная функция: {func.GetFunctionString()}\n", ConsoleColor.Yellow);
            Console.WriteLine("Интервалы интегрирования:");
            for (int i = 0; i < intervals.Count; i++)
                Console.WriteLine($"{i + 1}) [{FormatEDecimal(intervals[i].Item1)}; {FormatEDecimal(intervals[i].Item2)}]");

            if (eps != null)
                ColorPrint.Print($"\nТекущая точность: {eps}\n", ConsoleColor.Magenta);
            Console.WriteLine();
        }

        static void AwaitKey()
        {
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static void Exit()
        {
            Console.WriteLine("\nВыход из программы...");
            Environment.Exit(0);
        }

        static (bool success, string message, List<(EDecimal, EDecimal)> intervals) GetIntervalsToIntegrate(EDecimal a, EDecimal b, Function func)
        {
            List<DiscontinuityPoint> discontinuities = Math.Discontinuity.Utils.FindDiscontinuities(func, a, b);

            if (!Math.Discontinuity.Utils.IsIntegralConvergent(discontinuities))
            {
                var badPoints = discontinuities.Where(p => !p.IsConvergent).Select(p => FormatEDecimal(p.X));
                return (false, $"Интеграл расходится в точках: {string.Join(", ", badPoints)}", new List<(EDecimal, EDecimal)>());
            }
            var splitPoints = new List<EDecimal> { a, b };
            foreach (var p in discontinuities)
            {
                if (p.X.CompareTo(a) > 0 && p.X.CompareTo(b) < 0)
                    splitPoints.Add(p.X);
            }

            var sortedPoints = splitPoints.Distinct().OrderBy(x => x).ToList();
            var intervals = new List<(EDecimal, EDecimal)>();

            for (int i = 0; i < sortedPoints.Count - 1; i++)
            {
                EDecimal start = sortedPoints[i];
                EDecimal end = sortedPoints[i + 1];

                if (discontinuities.Any(d => (d.X - start).Abs().CompareTo(EDecimal.FromDouble(1e-12)) < 0))
                {
                    start = start.Add(GetOffset(start));
                }

                if (discontinuities.Any(d => (d.X - end).Abs().CompareTo(EDecimal.FromDouble(1e-12)) < 0))
                {
                    end = end.Subtract(GetOffset(end));
                }

                if (start.CompareTo(end) < 0)
                {
                    intervals.Add((start, end));
                }
            }

            string info = "Интеграл сходится";
            if (discontinuities.Any())
            {
                var details = discontinuities.Select(d => $"{FormatEDecimal(d.X)}");
                info += $". Обнаружены особенности: {string.Join("; ", details)}";
            }

            return (true, info, intervals);
        }

        private static EDecimal GetOffset(EDecimal value)
        {
            EDecimal minDelta = EDecimal.FromDouble(1e-3);
            EDecimal relativeDelta = value.Abs().Multiply(EDecimal.FromDouble(1e-8));
            return EDecimal.Max(minDelta, relativeDelta);
        }


        private static string FormatEDecimal(EDecimal value)
        {
            EDecimal eps = EDecimal.FromDouble(1e-10);
            if(value.Abs().CompareTo(eps) < 0)
            {
                return "0";
            }

            return value.ToDouble().ToString("F5");
        }
    }
}