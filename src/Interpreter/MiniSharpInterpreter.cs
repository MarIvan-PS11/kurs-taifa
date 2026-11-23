namespace MiniSharp.Interpreter;

using System.Collections.Generic;

using MiniSharp.Ast;
using MiniSharp.Execution;
using MiniSharp.Lexer;
using MiniSharp.Parser;

/// <summary>
/// Стабильный фасад интерпретатора: лексер → парсер → вычислитель.
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
        Parser parser = new Parser(tokens);
        ProgramNode program = parser.ParseProgram();

        AstEvaluator evaluator = new AstEvaluator(env);
        evaluator.Evaluate(program);
    }
}