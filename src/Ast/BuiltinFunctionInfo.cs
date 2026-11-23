namespace MiniSharp.Ast;

/// <summary>
/// Сигнатура встроенной функции: имя и количество аргументов.
/// Используется парсером для статической проверки вызовов.
/// </summary>
public sealed record BuiltinFunctionInfo(string Name, int Arity);