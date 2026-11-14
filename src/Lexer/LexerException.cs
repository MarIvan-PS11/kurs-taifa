namespace MiniSharp.Lexer;

/// <summary>
/// Исключение, возникающее при ошибках лексического анализа.
/// </summary>
public sealed class LexerException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр класса LexerException.
    /// </summary>
    public LexerException()
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса LexerException с указанным сообщением.
    /// </summary>
    public LexerException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса LexerException с сообщением и внутренним исключением.
    /// </summary>
    public LexerException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса LexerException с сообщением и позицией.
    /// </summary>
    public LexerException(string message, SourcePosition position)
        : base($"{message} (строка {position.Line}, столбец {position.Column})")
    {
        Position = position;
    }

    /// <summary>
    /// Позиция в коде, где произошла ошибка.
    /// </summary>
    public SourcePosition Position { get; } = new SourcePosition(0, 0);
}