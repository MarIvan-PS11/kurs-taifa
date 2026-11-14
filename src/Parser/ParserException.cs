namespace MiniSharp.Parser;

using MiniSharp.Lexer;

/// <summary>
/// Исключение, возникающее при ошибках синтаксического анализа.
/// </summary>
public sealed class ParserException : Exception
{
    public ParserException()
    {
    }

    public ParserException(string message)
        : base(message)
    {
    }

    public ParserException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public ParserException(string message, Token token)
        : base($"{message} (строка {token.Line}, столбец {token.Column})")
    {
        Position = new SourcePosition(token.Line, token.Column);
    }

    public SourcePosition Position { get; } = new SourcePosition(0, 0);
}