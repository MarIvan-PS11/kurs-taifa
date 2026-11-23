namespace MiniSharp.Ast;

using MiniSharp.Lexer;

/// <summary>
/// Базовый класс для всех узлов выражений.
/// </summary>
public abstract class Expression : AstNode
{
    /// <summary>
    /// Инициализирует выражение с указанной позицией.
    /// </summary>
    protected Expression(SourcePosition position)
        : base(position)
    {
    }
}