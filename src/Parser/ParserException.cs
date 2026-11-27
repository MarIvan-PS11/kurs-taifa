namespace MiniSharp.Parser;

using System;

using MiniSharp.Lexer;

/// <summary>
/// Исключение, возникающее при ошибках синтаксического анализа.
/// </summary>
public sealed class ParserException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр класса ParserException.
    /// </summary>
    public ParserException()
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса ParserException с указанным сообщением.
    /// </summary>
    public ParserException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса ParserException с сообщением и внутренним исключением.
    /// </summary>
    public ParserException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Инициализирует исключение с сообщением и токеном, где произошла ошибка.
    /// </summary>
    public ParserException(string message, Token token)
        : base($"{message} (строка {token.Line}, столбец {token.Column})")
    {
        Position = new SourcePosition(token.Line, token.Column);
    }

    /// <summary>
    /// Позиция в коде, где произошла ошибка.
    /// </summary>
    public SourcePosition Position { get; } = new SourcePosition(0, 0);
}