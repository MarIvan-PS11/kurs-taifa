namespace MiniSharp.Ast;

using System.Collections.Generic;

using MiniSharp.Lexer;

/// <summary>
/// Блок инструкций, заключённый в фигурные скобки.
/// </summary>
public sealed class BlockStatement : Statement
{
    /// <summary>
    /// Создаёт блок инструкций.
    /// </summary>
    public BlockStatement(IReadOnlyList<Statement> statements, SourcePosition position)
        : base(position)
    {
        Statements = statements;
    }

    /// <summary>
    /// Последовательность инструкций внутри блока.
    /// </summary>
    public IReadOnlyList<Statement> Statements { get; }

    /// <inheritdoc />
    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}