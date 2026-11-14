namespace MiniSharp.Interpreter;

using System;
using System.Collections.Generic;
using System.IO;

using MiniSharp.Lexer;
using MiniSharp.Parser;

/// <summary>
/// Точка входа консольного интерпретатора.
/// </summary>
public sealed class Program
{
    /// <summary>
    /// Запускает интерпретатор с реальной консолью.
    /// </summary>
    public static int Main(string[] args)
    {
        return Run(args, Console.In, Console.Out, Console.Error);
    }

    /// <summary>
    /// Запускает интерпретатор целиком с указанными потоками ввода, вывода и ошибок.
    /// </summary>
    public static int Run(string[] args, TextReader input, TextWriter output, TextWriter error)
    {
        if (args.Length != 1)
        {
            error.WriteLine("Ошибка: не указан путь к файлу с исходным кодом.");
            error.WriteLine("Использование: MiniSharp.Interpreter <путь_к_файлу>");
            return 1;
        }

        string filePath = args[0];
        if (!File.Exists(filePath))
        {
            error.WriteLine($"Ошибка: файл '{filePath}' не найден.");
            return 1;
        }

        try
        {
            string sourceCode = File.ReadAllText(filePath);
            Lexer lexer = new Lexer(sourceCode);
            List<Token> tokens = lexer.Tokenize();
            IEnvironment env = new ConsoleEnvironment(input, output);
            Parser parser = new Parser(tokens, env);
            parser.ParseProgram();
            return 0;
        }
        catch (LexerException ex)
        {
            error.WriteLine($"Ошибка лексического анализа: {ex.Message}");
            return 1;
        }
        catch (ParserException ex)
        {
            error.WriteLine($"Ошибка разбора: {ex.Message}");
            return 1;
        }
        catch (InterpreterException ex)
        {
            error.WriteLine($"Ошибка выполнения: {ex.Message}");
            return 1;
        }
        catch (Exception ex)
        {
            error.WriteLine($"Непредвиденная ошибка: {ex.Message}");
            return 1;
        }
    }
}