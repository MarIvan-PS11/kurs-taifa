namespace MiniSharp.Ast;

using MiniSharp.Lexer;

/// <summary>
/// Корневой узел программы: содержит объявление функции main.
/// </summary>
public sealed class ProgramNode : AstNode
{
    /// <summary>
    /// Создаёт корневой узел программы.
    /// </summary>
    public ProgramNode(FunctionDeclaration main, SourcePosition position)
        : base(position)
    {
        Main = main;
    }

    /// <summary>
    /// Объявление функции main.
    /// </summary>
    public FunctionDeclaration Main { get; }

    /// <inheritdoc />
    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}