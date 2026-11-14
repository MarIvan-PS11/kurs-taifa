namespace MiniSharp.Parser;

using MiniSharp.Lexer;

/// <summary>
/// Исключение, возникающее во время выполнения программы.
/// </summary>
public sealed class InterpreterException : Exception
{
    public InterpreterException()
    {
    }

    public InterpreterException(string message)
        : base(message)
    {
    }

    public InterpreterException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public InterpreterException(string message, Token token)
        : base($"{message} (строка {token.Line}, столбец {token.Column})")
    {
        Position = new SourcePosition(token.Line, token.Column);
    }

    public SourcePosition Position { get; } = new SourcePosition(0, 0);
}