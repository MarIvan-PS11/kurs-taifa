namespace MiniSharp.Ast;

using MiniSharp.Lexer;

/// <summary>
/// Базовый класс для всех узлов инструкций.
/// </summary>
public abstract class Statement : AstNode
{
    /// <summary>
    /// Инициализирует инструкцию с указанной позицией.
    /// </summary>
    protected Statement(SourcePosition position)
        : base(position)
    {
    }
}