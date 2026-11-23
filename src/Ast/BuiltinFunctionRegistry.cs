namespace MiniSharp.Ast;

using System.Collections.Generic;

/// <summary>
/// Реестр метаданных встроенных функций: имя и арность.
/// Реализации функций находятся в модуле Execution.
/// </summary>
public static class BuiltinFunctionRegistry
{
    private static readonly Dictionary<string, BuiltinFunctionInfo> Functions = new()
    {
        { "abs", new BuiltinFunctionInfo("abs", 1) },
        { "sqr", new BuiltinFunctionInfo("sqr", 1) },
        { "min", new BuiltinFunctionInfo("min", 2) },
        { "max", new BuiltinFunctionInfo("max", 2) },
    };

    /// <summary>
    /// Пытается получить метаданные встроенной функции по имени.
    /// </summary>
    public static bool TryGet(string name, out BuiltinFunctionInfo? info)
    {
        return Functions.TryGetValue(name, out info);
    }
}