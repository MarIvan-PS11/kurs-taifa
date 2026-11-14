namespace MiniSharp.Parser;

/// <summary>
/// Описание встроенной функции: имя, количество аргументов и логика вычисления.
/// </summary>
internal sealed record BuiltinFunction(string Name, int Arity, Func<List<long>, long> Implementation);