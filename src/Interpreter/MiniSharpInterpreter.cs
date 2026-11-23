namespace MiniSharp.Interpreter;

using MiniSharp.Lexer;
using MiniSharp.Parser;

/// <summary>
/// Стабильный фасад интерпретатора: лексер → парсер → вычисление.
/// Приёмочные тесты и консольный запуск вызывают только этот класс,
/// поэтому внутренние изменения конвейера не затрагивают тесты.
/// </summary>
public static class MiniSharpInterpreter
{
    /// <summary>
    /// Выполняет программу на MiniSharp в указанном окружении ввода-вывода.
    /// </summary>
    public static void Run(string source, IEnvironment env)
    {
        Lexer lexer = new Lexer(source);
        List<Token> tokens = lexer.Tokenize();
        Parser parser = new Parser(tokens, env);
        parser.ParseProgram();
    }
}