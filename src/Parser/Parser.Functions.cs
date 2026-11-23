namespace MiniSharp.Parser;

using MiniSharp.Lexer;

/// <summary>
/// Часть парсера, отвечающая за вызов встроенных функций.
/// </summary>
public sealed partial class Parser
{
    private long EvaluateBuiltinFunction(string funcName, List<long> args, Token token)
    {
        if (!BuiltinFunctions.TryGet(funcName, out BuiltinFunction? func) || func is null)
        {
            throw new ParserException($"Неизвестная встроенная функция '{funcName}'", token);
        }

        if (args.Count != func.Arity)
        {
            throw new ParserException($"Функция '{funcName}': Ожидаемое количество аргументов: {func.Arity}", token);
        }

        return EvaluateChecked(() => func.Implementation(args));
    }
}