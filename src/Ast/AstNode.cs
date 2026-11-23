namespace MiniSharp.Ast;

using MiniSharp.Lexer;

/// <summary>
/// Базовый класс всех узлов абстрактного синтаксического дерева.
/// </summary>
public abstract class AstNode
{
    /// <summary>
    /// Инициализирует узел с указанной позицией в исходном коде.
    /// </summary>
    protected AstNode(SourcePosition position)
    {
        Position = position;
    }

    /// <summary>
    /// Позиция узла в исходном коде для диагностики ошибок.
    /// </summary>
    public SourcePosition Position { get; }

    /// <summary>
    /// Принимает посетителя для обхода дерева.
    /// </summary>
    public abstract void Accept(IAstVisitor visitor);
}