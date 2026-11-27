namespace MiniSharp.Execution;

using System;

using MiniSharp.Lexer;

/// <summary>
/// Исключение, возникающее во время выполнения программы.
/// </summary>
public sealed class InterpreterException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр класса InterpreterException.
    /// </summary>
    public InterpreterException()
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса InterpreterException с указанным сообщением.
    /// </summary>
    public InterpreterException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса InterpreterException с сообщением и внутренним исключением.
    /// </summary>
    public InterpreterException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса InterpreterException с сообщением и позицией.
    /// </summary>
    public InterpreterException(string message, SourcePosition position)
        : base($"{message} (строка {position.Line}, столбец {position.Column})")
    {
        Position = position;
    }

    /// <summary>
    /// Позиция в коде, где произошла ошибка.
    /// </summary>
    public SourcePosition Position { get; } = new SourcePosition(0, 0);
}