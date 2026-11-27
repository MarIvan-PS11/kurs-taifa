namespace MiniSharp.Ast;

using System.Collections.Generic;

using MiniSharp.Lexer;

/// <summary>
/// Корневой узел AST, представляющий всю программу.
/// </summary>
public sealed class ProgramNode : AstNode
{
    /// <summary>
    /// Инициализирует новый экземпляр корневого узла программы.
    /// </summary>
    public ProgramNode(IReadOnlyList<FunctionDeclaration> functions, FunctionDeclaration main, SourcePosition position)
        : base(position)
    {
        Functions = functions;
        Main = main;
    }

    /// <summary>
    /// Список пользовательских функций, объявленных до main.
    /// </summary>
    public IReadOnlyList<FunctionDeclaration> Functions { get; }

    /// <summary>
    /// Объявление главной функции main.
    /// </summary>
    public FunctionDeclaration Main { get; }

    /// <inheritdoc />
    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}