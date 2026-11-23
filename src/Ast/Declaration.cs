namespace MiniSharp.Ast;

using MiniSharp.Lexer;

/// <summary>
/// Базовый класс для узлов объявлений (функций, переменных и т.п.).
/// </summary>
public abstract class Declaration : AstNode
{
    /// <summary>
    /// Инициализирует объявление с указанной позицией.
    /// </summary>
    protected Declaration(SourcePosition position)
        : base(position)
    {
    }
}