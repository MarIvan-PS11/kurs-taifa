namespace MiniSharp.Interpreter;

using System;
using System.IO;

using MiniSharp.Execution;

/// <summary>
/// Реализация окружения поверх текстовых потоков ввода и вывода.
/// </summary>
public sealed class ConsoleEnvironment : IEnvironment
{
    private readonly TextReader _input;
    private readonly TextWriter _output;

    /// <summary>
    /// Создаёт окружение с указанными потоками ввода и вывода.
    /// </summary>
    public ConsoleEnvironment(TextReader input, TextWriter output)
    {
        _input = input;
        _output = output;
    }

    /// <summary>
    /// Печатает целое число в поток вывода с переводом строки.
    /// </summary>
    public void Print(long value)
    {
        _output.WriteLine(value);
    }

    /// <summary>
    /// Читает целое число из потока ввода для встроенной функции readInt.
    /// </summary>
    public long ReadInt()
    {
        string? input = _input.ReadLine();
        if (input == null)
        {
            throw new InterpreterException("Неожиданный конец потока ввода при вызове readInt()");
        }

        input = input.Trim();
        if (!long.TryParse(input, out long result))
        {
            throw new InterpreterException($"Не удалось прочитать целое число из ввода: '{input}'");
        }

        return result;
    }
}