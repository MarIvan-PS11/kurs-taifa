namespace MiniSharp.Execution
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Реализации встроенных функций языка. Отделены от парсера и метаданных.
    /// </summary>
    internal static class BuiltinImplementations
    {
        /// <summary>
        /// Реестр реализаций: имя функции → функция вычисления.
        /// </summary>
        private static readonly Dictionary<string, Func<IReadOnlyList<long>, long>> Implementations = new()
        {
            { "abs", args => checked(Math.Abs(args[0])) },
            { "sqr", args => checked(args[0] * args[0]) },
            { "min", args => Math.Min(args[0], args[1]) },
            { "max", args => Math.Max(args[0], args[1]) },
        };

        /// <summary>
        /// Пытается получить реализацию встроенной функции по её имени.
        /// </summary>
        public static bool TryGet(string name, out Func<IReadOnlyList<long>, long>? implementation)
        {
            return Implementations.TryGetValue(name, out implementation);
        }
    }
}