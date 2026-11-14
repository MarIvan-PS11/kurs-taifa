namespace MiniSharp.Parser;

/// <summary>
/// Реестр встроенных функций языка MiniSharp.
/// </summary>
internal static class BuiltinFunctions
{
    private static readonly Dictionary<string, BuiltinFunction> Functions = new()
    {
        { "abs", new BuiltinFunction("abs", 1, args => checked(Math.Abs(args[0]))) },
        { "sqr", new BuiltinFunction("sqr", 1, args => checked(args[0] * args[0])) },
        { "min", new BuiltinFunction("min", 2, args => Math.Min(args[0], args[1])) },
        { "max", new BuiltinFunction("max", 2, args => Math.Max(args[0], args[1])) },
    };

    /// <summary>
    /// Пытается получить описание встроенной функции по её имени.
    /// </summary>
    public static bool TryGet(string name, out BuiltinFunction? function)
    {
        return Functions.TryGetValue(name, out function);
    }
}