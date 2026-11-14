namespace MiniSharp.Interpreter.AcceptanceTests;

using System;
using System.Collections.Generic;
using System.Text;

using MiniSharp.Parser;

/// <summary>
/// Тестовый дублёр (Fake) для перехвата ввода и вывода интерпретатора.
/// </summary>
public sealed class FakeEnvironment : IEnvironment
{
    private readonly Queue<long> _inputValues;
    private readonly StringBuilder _output = new();

    public FakeEnvironment(IEnumerable<long>? inputValues = null)
    {
        _inputValues = new Queue<long>(inputValues ?? Array.Empty<long>());
    }

    public void Print(long value)
    {
        _output.Append(value.ToString() + "\n");
    }

    public long ReadInt()
    {
        if (_inputValues.Count == 0)
        {
            throw new InterpreterException("Неожиданный конец потока ввода при вызове readInt()");
        }

        return _inputValues.Dequeue();
    }

    /// <summary>
    /// Возвращает весь накопленный вывод интерпретатора.
    /// </summary>
    public string GetOutput()
    {
        return _output.ToString();
    }
}