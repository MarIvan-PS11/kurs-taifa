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
            throw new ParserException($"Функция '{funcName}' требует ровно {func.Arity} аргумента", token);
        }

        try
        {
            return func.Implementation(args);
        }
        catch (OverflowException)
        {
            throw new InterpreterException($"Арифметическое переполнение в функции '{funcName}'", token);
        }
    }
}